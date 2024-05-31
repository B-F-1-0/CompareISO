namespace CompareISO
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Management;
    using System.Management.Automation;

    using System.Diagnostics;
    using Microsoft.Dism;
    using DiscUtils.Iso9660;
    using DiscUtils;
    using DiscUtils.Udf;
    using ManagedWimLib;
    using System.Collections.ObjectModel;
    using static Microsoft.Wim.WimgApi;
    using Newtonsoft.Json.Linq;

    // using SevenZipExtractor;
    public partial class Form1 : Form
    {
        // iso directories
        const String iso1Directory = "ISO1Contents";
        const String iso2Directory = "ISO2Contents";

        // hardcode directories
        const String WIMdetect = @"\\SOURCES\\INSTALL.WIM";
        const String i386detect = @"\\I386";
        const String amd64detect = @"\\AMD64";
        static readonly string[] win9xdetect = { "\\WIN95", "\\WIN9X", "\\WIN98", "\\WINME", "\\RETAIL" };

        // safe keepings for removed and added table output
        List<string> removedFilesTableFormat = new List<string>();
        List<string> addedFilesTableFormat = new List<string>();

        public Form1()
        {
            InitializeComponent();
            Wim.GlobalInit("runtimes\\win-x64\\native\\libwim-15.dll");
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ISO1button_Click(object sender, EventArgs e)
        {
            try
            {
                if (openISO1FileDialog.ShowDialog() == DialogResult.OK)
                {
                    iso1FileNameTextBox.Text = openISO1FileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred. " + ex.Message);
                return;
            }
        }

        private void ISO2button_Click(object sender, EventArgs e)
        {
            try
            {
                if (openISO1FileDialog.ShowDialog() == DialogResult.OK)
                {
                    iso2FileNameTextBox.Text = openISO1FileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred. " + ex.Message);
                return;
            }
        }

        private void compareButton_Click(object sender, EventArgs e)
        {
            // time to compare, disable buttons to prevent disruptions
            addedFilesRichTextBox.Clear();
            removedFilesRichTextBox.Clear();
            ISO1button.Enabled = false;
            ISO2button.Enabled = false;
            compareButton.Enabled = false;
            wikiTableButton.Enabled = false;
            exitButton.Enabled = false;
            extractProgressBar.Value = 0;
            extractProgressBar.ForeColor = Color.OliveDrab;
            removedFilesTableFormat.Clear();
            addedFilesTableFormat.Clear();

            // set up variables
            String iso1Path = @iso1FileNameTextBox.Text;
            String iso2Path = @iso2FileNameTextBox.Text;
            bool isWim = false;

            progressLabel.Text = "Checking ISOs...";

            // validate files
            if (!validateIsos())
            {
                return; // if validation fails (is false), abort
            }

            // try loading files
            try
            {
                using (FileStream test = new FileStream(iso1Path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    test.Close();
                }
            }
            catch
            {
                abort();
                MessageBox.Show("Unable to open ISO files. They might be in use by another program. Please close any programs using the ISOs and try again.");
                return;
            }
            try
            {
                using (FileStream test = new FileStream(iso2Path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    test.Close();
                }
            }
            catch
            {
                abort();
                MessageBox.Show("Unable to open ISO files. They might be in use by another program. Please close any programs using the ISOs and try again.");
                return;
            }

            // if we made it here, it is safe to open the files
            FileStream iso1FileStream = new FileStream(iso1Path, FileMode.Open, FileAccess.Read, FileShare.None);
            FileStream iso2FileStream = new FileStream(iso2Path, FileMode.Open, FileAccess.Read, FileShare.None);

            // make and clean directories
            progressLabel.Text = "Creating/cleaning temporary directories...";
            checkDirectories();

            // try to identify the type of install media to make life easier
            progressLabel.Text = "Detecting versions...";
            try
            {
                switch (detectWindows(iso1FileStream, iso2FileStream))
                {
                    case "win9x":
                        {
                            MessageBox.Show("This program has detected that it's trying to compare two Windows 9x install disks.\n\nThe contents of the RETAIL/WIN95/WIN9X/WIN98/WINME folders will be compared, and all CAB files will be expanded.");
                            compare9xFiles(iso1FileStream, iso2FileStream);
                            break;
                        }
                    case "i386":
                        {
                            MessageBox.Show("This program has detected that it's trying to compare two Windows NT install disks with an I386 folder.\n\nThe contents of the I386 folders will be compared, but not its subdirectories. If there are unexpanded files in the directory, it will expand those and remove the unexpanded versions.");
                            comparei386Files(iso1FileStream, iso2FileStream);
                            break;
                        }
                    case "amd64":
                        {
                            MessageBox.Show("This program has detected that it's trying to compare two Windows NT install disks with an I386 and AMD64 folder.\n\nThe contents of the I386 and AMD64 folders will be compared, but not its subdirectories. If there are unexpanded files in the directory, it will expand those and remove the unexpanded versions.");
                            compareamd64Files(iso1FileStream, iso2FileStream);
                            break;
                        }
                    case "wim":
                        {
                            MessageBox.Show("This program has detected that it's trying to compare two Windows NT install disks with an INSTALL.WIM file.\n\nINSTALL.WIM will be extracted, and then you may choose an index in the WIM to extract. There might be inaccuracies if the SKUs don't match. Currently, fetching the file description and file version is unsupported.");
                            compareWIMFiles(iso1FileStream, iso2FileStream);
                            isWim = true;
                            break;
                        }
                    case "wimufi":
                        {
                            MessageBox.Show("This program has detected that it's trying to compare two Windows NT install disks with an INSTALL.WIM file, and the ISOs are in UDF (UFI bootable) format.\n\nINSTALL.WIM will now be extracted, and then you may choose an index in the WIM to extract. There might be inaccuracies if the SKUs don't match. Currently, fetching the file description and file version is unsupported.");
                            compareWIMUFIFiles(iso1FileStream, iso2FileStream);
                            isWim = true;
                            break;
                        }
                    default:
                        {
                            MessageBox.Show("This program was unable to determine the type of disks to compare, or both disks don't use the same format.\n\nThe contents of all files (including those in subdirectories) will be compared, but there might be inaccuracies. If the ISO image is complex or large, the program may take a long time to extract everything; please be patient. If you have not seen any progress after several minutes, terminate this program using Task Manager.");
                            compareAllFiles(iso1FileStream, iso2FileStream);
                            break;
                        }
                }

                // save the file versions given the inputted string list, and store them in the rich text box for the new form
                progressLabel.Text = "Saving file version and description information...";
                saveVersionInfo(isWim);

                // done; message if they want to clean up temp files
                iso1FileStream.Close();
                iso2FileStream.Close();
                successMessage();
            }
            catch (Exception ex)
            {
                iso1FileStream.Close();
                iso2FileStream.Close();
                abort();
                MessageBox.Show("An error has occurred. " + ex.Message);
            }
        }

        private String detectWindows(FileStream iso1, FileStream iso2)
        {
            // this function checks the contents of the folders to ensure they're of a specific type
            // since only ISO is supported, this won't work for DOS Windows, except 3.0 MME
            // WARNING: High Sierra format is currently unsupported; most disks below Windows 95
            //          may not work!
            //          If your ISO is in that format, you may have to convert it to ISO-9660
            //          (CDFS) first.

            // the best way we can identify the Windows versions is by finding the folowing directories/files:
            // Windows 9x: RETAIL, WIN9X, WIN95, WIN98, WINME
            // Windows NT (i386 era): I386
            // Windows NT (x64): AMD64
            // Windows NT (WIM era): SOURCES/INSTALL.WIM
            // If all else fails, we didn't recognize it
            // Anything of a non-Windows installation is unsupported

            String iso1Format = "unknown";
            String iso2Format = "unknown";
            String[] directories = null;
            String[] sourcesfiles = null;
            String[] files = null;

            // iso1 check
            try
            {
                CDReader cd = new CDReader(iso1, true);
                directories = cd.GetDirectories(@"");
                directories = directories.Select(s => s.ToUpper()).ToArray();
                files = cd.GetFiles(@"");
                
                if (directories.Contains("\\SOURCES"))
                {
                    sourcesfiles = cd.GetFiles("\\SOURCES");
                    sourcesfiles = sourcesfiles.Select(s => s.ToUpper()).ToArray();
                    if (sourcesfiles.Contains("\\SOURCES\\INSTALL.WIM"))
                    {
                        iso1Format = "wim";
                    }
                }
                else if (directories.Contains("\\I386"))
                {
                    if (directories.Contains("\\AMD64"))
                    {
                        iso1Format = "amd64";
                    }
                    else
                    {
                        iso1Format = "i386";
                    }
                }
                else if (win9xdetect.Intersect(directories).Any())
                {
                    iso1Format = "win9x";
                }
            }
            catch
            {
                iso1Format = "unknown";
            }

            // iso2 check
            try
            {
                CDReader cd = new CDReader(iso2, true);
                directories = cd.GetDirectories(@"");
                directories = directories.Select(s => s.ToUpper()).ToArray();
                files = cd.GetFiles(@"");

                if (directories.Contains("\\SOURCES"))
                {
                    sourcesfiles = cd.GetFiles("\\SOURCES");
                    sourcesfiles = sourcesfiles.Select(s => s.ToUpper()).ToArray();
                    if (sourcesfiles.Contains("\\SOURCES\\INSTALL.WIM"))
                    {
                        iso2Format = "wim";
                    }
                }
                else if (directories.Contains("\\I386"))
                {
                    if (directories.Contains("\\AMD64"))
                    {
                        iso2Format = "amd64";
                    }
                    else
                    {
                        iso2Format = "i386";
                    }
                }
                else if (win9xdetect.Intersect(directories).Any())
                {
                    iso2Format = "win9x";
                }
            }
            catch
            {
                iso2Format = "unknown";
            }

            // the above won't read udf isos by default; the below is a last resort to see if it will
            // detect it and if such detect it as wim
            if (iso1Format == "unknown" && iso2Format == "unknown")
            {
                try
                {
                    UdfReader iso1ufi = new UdfReader(iso1);
                    if (iso1ufi.FileExists("\\SOURCES\\INSTALL.WIM"))
                    {
                        iso1Format = "wimufi";
                    }
                    else
                    {
                        iso1Format = "unknown";
                    }
                }
                catch
                {
                    iso1Format = "unknown";
                }
                try
                {
                    UdfReader iso2ufi = new UdfReader(iso2);
                    if (iso2ufi.FileExists("\\SOURCES\\INSTALL.WIM"))
                    {
                        iso2Format = "wimufi";
                    }
                    else
                    {
                        iso2Format = "unknown";
                    }
                }
                catch
                {
                    iso2Format = "unknown";
                }
            }

            if (iso1Format == iso2Format)
            {
                return iso1Format;
            }
            else
            {
                return "unknown";
            }
        }

        private void abort()
        {
            // this function re-enables all buttons; useful if the program gets stumped
            // however, the progress bar will be red to indicate a failure
            ISO1button.Enabled = true;
            ISO2button.Enabled = true;
            compareButton.Enabled = true;
            wikiTableButton.Enabled = true;
            exitButton.Enabled = true;
            extractProgressBar.ForeColor = Color.DarkRed;
            // update progress
            progressLabel.Text = "Comparison failed";
        }

        static void checkDirectories()
        {
            // this function makes the ISOContents directory in the same directory this app is in, and cleans up them if necessary
            String errortext = "";
            String titletext = "Directory not empty";
            Form2 error = new Form2();

            //String iso1Directory = "ISO1Contents";
            //String iso2Directory = "ISO2Contents";

            Directory.CreateDirectory(iso1Directory);
            Directory.CreateDirectory(iso2Directory);

            // if these directories above aren't empty, prompt to delete the contents in there
            System.IO.DirectoryInfo dir1 = new DirectoryInfo(iso1Directory);
            System.IO.DirectoryInfo dir2 = new DirectoryInfo(iso2Directory);

            if (Directory.EnumerateFileSystemEntries(iso1Directory).Any())
            {
                errortext = "The directory " + iso1Directory + " is not empty; do you want to delete the files in there first?";
                error.changeError(errortext, titletext);
                if (error.ShowDialog() == DialogResult.Yes)
                {
                    foreach (FileInfo file in dir1.EnumerateFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in dir1.EnumerateDirectories())
                    {
                        dir.Delete(true);
                    }
                }
            }
            if (Directory.EnumerateFileSystemEntries(iso2Directory).Any())
            {
                errortext = "The directory " + iso2Directory + " is not empty; do you want to delete the files in there first?";
                error.changeError(errortext, titletext);
                if (error.ShowDialog() == DialogResult.Yes)
                {
                    foreach (FileInfo file in dir2.EnumerateFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in dir2.EnumerateDirectories())
                    {
                        dir.Delete(true);
                    }
                }
            }

            // done cleaning, let's finish up
            return;
        }

        private void compareAllFiles(FileStream iso1, FileStream iso2) // compares all files including subdirectories; should land here is unknown format(s)
        {
            // extract both ISOs and put them in the extracted directory created
            // we should do this recursively
            extractProgressBar.Value = 0;
            extractProgressBar.Maximum = 100;

            progressLabel.Text = "Extracting ISO 1...";
            using (iso1)
            {
                CDReader cd = new CDReader(iso1, true);
                extractRecursive("", cd.GetDirectories(""), iso1,iso1Directory);
            }
            extractProgressBar.Value = 50;
            extractProgressBar.Value = 49;
            extractProgressBar.Value = 50;

            progressLabel.Text = "Extracting ISO 2...";
            using (iso2)
            {
                CDReader cd = new CDReader(iso2, true);
                extractRecursive("", cd.GetDirectories(""), iso2, iso2Directory);
            }

            extractProgressBar.Value = 100;

            // find what's in the ISO1Directory but not in ISO2Directory, and put them in removed files
            string[] iso1Files = Directory.GetFiles(iso1Directory, "*", SearchOption.AllDirectories);
            string[] iso2Files = Directory.GetFiles(iso2Directory, "*", SearchOption.AllDirectories);

            for (int i = 0; i < iso1Files.Length; i++)
            {
                iso1Files[i] = iso1Files[i].Remove(0, iso1Directory.Length + 1);
            }
            for (int i = 0; i < iso2Files.Length; i++)
            {
                iso2Files[i] = iso2Files[i].Remove(0, iso2Directory.Length + 1);
            }

            // resolve case sensativity issues
            iso1Files = iso1Files.Select(s => s.ToLowerInvariant()).ToArray();
            iso2Files = iso2Files.Select(s => s.ToLowerInvariant()).ToArray();

            IEnumerable<String> removedFiles = iso1Files.Except(iso2Files);
            IEnumerable<String> addedFiles = iso2Files.Except(iso1Files);

            progressLabel.Text = "Comparing files...";
            foreach (String s in removedFiles)
            {
                removedFilesRichTextBox.ReadOnly = false;
                removedFilesRichTextBox.AppendText(s + "\n");
                removedFilesRichTextBox.ReadOnly = true;
            }
            foreach (String s in addedFiles)
            {
                addedFilesRichTextBox.ReadOnly = false;
                addedFilesRichTextBox.AppendText(s + "\n");
                addedFilesRichTextBox.ReadOnly = true;
            }
            
        }

        private String[] extractRecursive(String currentDir, String[] directories, FileStream iso, String saveDirectory)
        {
            // scan directory and find if there are more
            // if so, go to that one and continue until we reach the end
            // once the end if reached, extract them
            CDReader cd = new CDReader(iso, true);
            if (!directories.Any())
            {
                String[] files = cd.GetFiles(currentDir);
                if (!files.Any())
                {
                    return new string[0];
                }
                foreach (String f in files) // for each directory
                {
                    copyFile(cd.GetFileInfo(f), saveDirectory);
                }
                return new string[0];
            }
            else
            {
                String[] additionalDirs = cd.GetDirectories(currentDir);
                foreach (String d in directories) // for each directory
                {
                    extractRecursive(d, additionalDirs, iso, saveDirectory);
                }
                
            }
            return new string[0];
        }

        private void comparei386Files(FileStream iso1, FileStream iso2) // extracts all files in I386, then compares them
        {
            // extract contents of I386 folder of both ISOs and puts them in the extracted directory created
            progressLabel.Text = "Extracting ISO 1...";
            using (iso1)
            {
                CDReader cd = new CDReader(iso1, true);
                String[] files = cd.GetFiles("\\I386");
                extractProgressBar.Value = 0;
                extractProgressBar.Maximum = files.Length;
                foreach (String s in files)
                {
                    copyFile(cd.GetFileInfo(s), iso1Directory);
                    extractProgressBar.Value++;
                }
            }

            progressLabel.Text = "Extracting ISO 2...";
            using (iso2)
            {
                CDReader cd = new CDReader(iso2, true);
                String[] files = cd.GetFiles("\\I386");
                extractProgressBar.Value = 0;
                extractProgressBar.Maximum = files.Length;
                foreach (String s in files)
                {
                    copyFile(cd.GetFileInfo(s), iso2Directory);
                    extractProgressBar.Value++;
                }
            }

            // in both directories, check for files ending with _ or CAB and expand them
            extractCABs();

            // find what's in the ISO1Directory but not in ISO2Directory, and put them in removed files
            string[] iso1Files = Directory.GetFiles(iso1Directory, "*", SearchOption.TopDirectoryOnly);
            string[] iso2Files = Directory.GetFiles(iso2Directory, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < iso1Files.Length; i++)
            {
                iso1Files[i] = iso1Files[i].Remove(0, iso1Directory.Length + 1);
            }
            for (int i = 0; i < iso2Files.Length; i++)
            {
                iso2Files[i] = iso2Files[i].Remove(0, iso2Directory.Length + 1);
            }

            // resolve case sensativity issues
            iso1Files = iso1Files.Select(s => s.ToLowerInvariant()).ToArray();
            iso2Files = iso2Files.Select(s => s.ToLowerInvariant()).ToArray();

            IEnumerable<String> removedFiles = iso1Files.Except(iso2Files);
            IEnumerable<String> addedFiles = iso2Files.Except(iso1Files);

            progressLabel.Text = "Comparing files...";
            foreach (String s in removedFiles)
            {
                removedFilesRichTextBox.ReadOnly = false;
                removedFilesRichTextBox.AppendText(s + "\n");
                removedFilesRichTextBox.ReadOnly = true;
            }
            foreach (String s in addedFiles)
            {
                addedFilesRichTextBox.ReadOnly = false;
                addedFilesRichTextBox.AppendText(s + "\n");
                addedFilesRichTextBox.ReadOnly = true;
            }
        }

        private void compareamd64Files(FileStream iso1, FileStream iso2) // extracts all files in I386/AMD64, then compares them
        {
            // extract contents of I386/AND64 folder of both ISOs and puts them in the extracted directory created
            progressLabel.Text = "Extracting ISO 1...";
            using (iso1)
            {
                CDReader cd = new CDReader(iso1, true);
                String[] files = cd.GetFiles("\\I386");
                extractProgressBar.Value = 0;
                extractProgressBar.Maximum = files.Length;
                foreach (String s in files)
                {
                    copyFile(cd.GetFileInfo(s), iso1Directory);
                    extractProgressBar.Value++;
                }
                files = cd.GetFiles("\\AMD64");
                extractProgressBar.Value = 0;
                extractProgressBar.Maximum = files.Length;
                foreach (String s in files)
                {
                    copyFile(cd.GetFileInfo(s), iso1Directory);
                    extractProgressBar.Value++;
                }
            }

            progressLabel.Text = "Extracting ISO 2...";
            using (iso2)
            {
                CDReader cd = new CDReader(iso2, true);
                String[] files = cd.GetFiles("\\I386");
                extractProgressBar.Value = 0;
                extractProgressBar.Maximum = files.Length;
                foreach (String s in files)
                {
                    copyFile(cd.GetFileInfo(s), iso2Directory);
                    extractProgressBar.Value++;
                }
                files = cd.GetFiles("\\AMD64");
                extractProgressBar.Value = 0;
                extractProgressBar.Maximum = files.Length;
                foreach (String s in files)
                {
                    copyFile(cd.GetFileInfo(s), iso2Directory);
                    extractProgressBar.Value++;
                }
            }

            // in both directories, check for files ending with _ or CAB and expand them
            extractCABs();

            // find what's in the ISO1Directory but not in ISO2Directory, and put them in removed files
            string[] iso1Files = Directory.GetFiles(iso1Directory, "*", SearchOption.TopDirectoryOnly);
            string[] iso2Files = Directory.GetFiles(iso2Directory, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < iso1Files.Length; i++)
            {
                iso1Files[i] = iso1Files[i].Remove(0, iso1Directory.Length + 1);
            }
            for (int i = 0; i < iso2Files.Length; i++)
            {
                iso2Files[i] = iso2Files[i].Remove(0, iso2Directory.Length + 1);
            }

            // resolve case sensativity issues
            iso1Files = iso1Files.Select(s => s.ToLowerInvariant()).ToArray();
            iso2Files = iso2Files.Select(s => s.ToLowerInvariant()).ToArray();

            IEnumerable<String> removedFiles = iso1Files.Except(iso2Files);
            IEnumerable<String> addedFiles = iso2Files.Except(iso1Files);

            progressLabel.Text = "Comparing files...";
            foreach (String s in removedFiles)
            {
                removedFilesRichTextBox.ReadOnly = false;
                removedFilesRichTextBox.AppendText(s + "\n");
                removedFilesRichTextBox.ReadOnly = true;
            }
            foreach (String s in addedFiles)
            {
                addedFilesRichTextBox.ReadOnly = false;
                addedFilesRichTextBox.AppendText(s + "\n");
                addedFilesRichTextBox.ReadOnly = true;
            }
        }

        private void compare9xFiles(FileStream iso1, FileStream iso2) // extracts all files in a WIN9X install folder, then compares them
        {
            String iso19xdir = "WIN9X";
            String iso29xdir = "WIN9X";

            progressLabel.Text = "Extracting ISO 1...";
            CDReader cd1 = new CDReader(iso1, true);
            String[] directories = cd1.GetDirectories(@"");
            directories = directories.Select(s => s.ToUpper()).ToArray();
            iso19xdir = win9xdetect.Intersect(directories).First();

            progressLabel.Text = "Extracting ISO 2...";
            CDReader cd2 = new CDReader(iso2, true);
            directories = cd2.GetDirectories(@"");
            directories = directories.Select(s => s.ToUpper()).ToArray();
            iso29xdir = win9xdetect.Intersect(directories).First();

            // after we find the correct directories, extract them
            using (iso1)
            {
                CDReader cd = new CDReader(iso1, true);
                String[] files1 = cd.GetFiles(iso19xdir);
                files1 = files1.Select(x => x.Replace(";1", string.Empty)).ToArray();
                extractProgressBar.Value = 0;
                extractProgressBar.Maximum = files1.Length;
                foreach (String s in files1)
                {
                    copyFile(cd.GetFileInfo(s), iso1Directory);
                    extractProgressBar.Value++;
                }
            }

            using (iso2)
            {
                CDReader cd = new CDReader(iso2, true);
                String[] files2 = cd.GetFiles(iso29xdir);
                files2 = files2.Select(x => x.Replace(";1", string.Empty)).ToArray();
                extractProgressBar.Value = 0;
                extractProgressBar.Maximum = files2.Length;
                foreach (String s in files2)
                {
                    copyFile(cd.GetFileInfo(s), iso2Directory);
                    extractProgressBar.Value++;
                }
            }

            // in both directories, check for files ending with _ or CAB and expand them
            extractCABs();

            // find what's in the ISO1Directory but not in ISO2Directory, and put them in removed files
            string[] iso1Files = Directory.GetFiles(iso1Directory, "*", SearchOption.TopDirectoryOnly);
            string[] iso2Files = Directory.GetFiles(iso2Directory, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < iso1Files.Length; i++)
            {
                iso1Files[i] = iso1Files[i].Remove(0, iso1Directory.Length + 1);
            }
            for (int i = 0; i < iso2Files.Length; i++)
            {
                iso2Files[i] = iso2Files[i].Remove(0, iso2Directory.Length + 1);
            }

            // resolve case sensativity issues
            iso1Files = iso1Files.Select(s => s.ToLowerInvariant()).ToArray();
            iso2Files = iso2Files.Select(s => s.ToLowerInvariant()).ToArray();

            IEnumerable<String> removedFiles = iso1Files.Except(iso2Files);
            IEnumerable<String> addedFiles = iso2Files.Except(iso1Files);

            progressLabel.Text = "Comparing files...";
            foreach (String s in removedFiles)
            {
                removedFilesRichTextBox.ReadOnly = false;
                removedFilesRichTextBox.AppendText(s + "\n");
                removedFilesRichTextBox.ReadOnly = true;
            }
            foreach (String s in addedFiles)
            {
                addedFilesRichTextBox.ReadOnly = false;
                addedFilesRichTextBox.AppendText(s + "\n");
                addedFilesRichTextBox.ReadOnly = true;
            }
        }

        
        private void compareWIMFiles(FileStream iso1, FileStream iso2) // extracts install.wim, then compares the contents of a selected index inside there
        {
            extractProgressBar.Maximum = 100;
            progressLabel.Text = "Extracting ISO 1's INSTALL.WIM...";
            using (iso1)
            {
                CDReader cd = new CDReader(iso1, true);
                copyFile(cd.GetFileInfo("\\SOURCES\\INSTALL.WIM"), iso1Directory);
            }
            extractProgressBar.Value = 50;
            extractProgressBar.Value = 49;
            extractProgressBar.Value = 50;
            progressLabel.Text = "Extracting ISO 2's INSTALL.WIM...";
            using (iso2)
            {
                CDReader cd = new CDReader(iso2, true);
                copyFile(cd.GetFileInfo("\\SOURCES\\INSTALL.WIM"), iso2Directory);
            }
            extractProgressBar.Value = 100;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                // Get the images in the WIM
                progressLabel.Text = "Requesting SKUs...";
                DismImageInfoCollection imageInfo1 = DismApi.GetImageInfo(iso1Directory + "\\INSTALL.WIM");
                DismImageInfoCollection imageInfo2 = DismApi.GetImageInfo(iso2Directory + "\\INSTALL.WIM");

                Form4 chooseIndexPrompt = new Form4();

                chooseIndexPrompt.addEntries(imageInfo1, 1);
                chooseIndexPrompt.addEntries(imageInfo2, 2);

                chooseIndexPrompt.ShowDialog();
                DismApi.Shutdown();

                var powerShell = PowerShell.Create();

                // add the functions we need
                progressLabel.Text = "Storing filenames to temporary text file...";
                String wim1command = "Get-WindowsImageContent -ImagePath \"" + iso1Directory + "\\install.wim\"" + " -Index " + chooseIndexPrompt.indexwim1;
                String wim2command = "Get-WindowsImageContent -ImagePath \"" + iso2Directory + "\\install.wim\"" + " -Index " + chooseIndexPrompt.indexwim2;
                powerShell.AddScript(wim1command);
                Collection<PSObject> PSOutput1 = powerShell.Invoke(); // write the files we find to a collection
                foreach (PSObject obj in PSOutput1)
                {
                    String fullfilename = obj.ToString(); // split filenames and only get the end value
                    String[] filename = fullfilename.Split("\\");
                    File.AppendAllText(iso1Directory + "\\FILES.TXT", "\n" + filename.Last());
                }

                powerShell.AddScript(wim2command);
                Collection<PSObject> PSOutput2 = powerShell.Invoke(); // write the files we find to a collection
                foreach (PSObject obj in PSOutput2)
                {
                    String fullfilename = obj.ToString(); // split filenames and only get the end value
                    String[] filename = fullfilename.Split("\\");
                    File.AppendAllText(iso2Directory + "\\FILES.TXT", "\n" + filename.Last());
                }

                var logFileIso1 = File.ReadAllLines(iso1Directory + "\\FILES.TXT");
                var log1List = new List<string>(logFileIso1);
                var logFileIso2 = File.ReadAllLines(iso2Directory + "\\FILES.TXT");
                var log2List = new List<string>(logFileIso2);

                // resolve case sensativity issues
                log1List = log1List.ConvertAll(d => d.ToLower());
                log2List = log2List.ConvertAll(d => d.ToLower());

                IEnumerable<String> removedFiles = log1List.Except(log2List);
                IEnumerable<String> addedFiles = log2List.Except(log1List);

                progressLabel.Text = "Comparing files...";
                foreach (String s in removedFiles)
                {
                    removedFilesRichTextBox.ReadOnly = false;
                    removedFilesRichTextBox.AppendText(s + "\n");
                    removedFilesRichTextBox.ReadOnly = true;
                }
                foreach (String s in addedFiles)
                {
                    addedFilesRichTextBox.ReadOnly = false;
                    addedFilesRichTextBox.AppendText(s + "\n");
                    addedFilesRichTextBox.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to compare files. This can be caused due to a corrupt INSTALL.WIM, or older INSTALL.WIM versions that aren't supported by DISM. Most builds of Windows Vista are currently not supported. The error was: " + ex.Message);
                DismApi.Shutdown();
            }
        }

        private void compareWIMUFIFiles(FileStream iso1, FileStream iso2) // extracts install.wim, then compares the contents of a selected index inside there
        {
            extractProgressBar.Maximum = 100;
            progressLabel.Text = "Extracting ISO 1's INSTALL.WIM...";
            using (iso1)
            {
                UdfReader cd = new UdfReader(iso1);
                copyFile(cd.GetFileInfo("\\SOURCES\\INSTALL.WIM"), iso1Directory);
            }
            extractProgressBar.Value = 50;
            extractProgressBar.Value = 49;
            extractProgressBar.Value = 50;
            progressLabel.Text = "Extracting ISO 2's INSTALL.WIM...";
            using (iso2)
            {
                UdfReader cd = new UdfReader(iso2);
                copyFile(cd.GetFileInfo("\\SOURCES\\INSTALL.WIM"), iso2Directory);
            }
            extractProgressBar.Value = 100;

            try
            {
                DismApi.Initialize(DismLogLevel.LogErrors);
                // Get the images in the WIM
                progressLabel.Text = "Requesting SKUs...";
                DismImageInfoCollection imageInfo1 = DismApi.GetImageInfo(iso1Directory + "\\INSTALL.WIM");
                DismImageInfoCollection imageInfo2 = DismApi.GetImageInfo(iso2Directory + "\\INSTALL.WIM");

                Form4 chooseIndexPrompt = new Form4();

                chooseIndexPrompt.addEntries(imageInfo1, 1);
                chooseIndexPrompt.addEntries(imageInfo2, 2);

                chooseIndexPrompt.ShowDialog();
                DismApi.Shutdown();

                var powerShell = PowerShell.Create();

                // add the functions we need
                progressLabel.Text = "Storing filenames to temporary text file...";
                String wim1command = "Get-WindowsImageContent -ImagePath \"" + iso1Directory + "\\install.wim\"" + " -Index " + chooseIndexPrompt.indexwim1;
                String wim2command = "Get-WindowsImageContent -ImagePath \"" + iso2Directory + "\\install.wim\"" + " -Index " + chooseIndexPrompt.indexwim2;
                powerShell.AddScript(wim1command);
                Collection<PSObject> PSOutput1 = powerShell.Invoke(); // write the files we find to a collection
                foreach (PSObject obj in PSOutput1)
                {
                    String fullfilename = obj.ToString(); // split filenames and only get the end value
                    String[] filename = fullfilename.Split("\\");
                    File.AppendAllText(iso1Directory + "\\FILES.TXT", "\n" + filename.Last());
                }

                powerShell.AddScript(wim2command);
                Collection<PSObject> PSOutput2 = powerShell.Invoke(); // write the files we find to a collection
                foreach (PSObject obj in PSOutput2)
                {
                    String fullfilename = obj.ToString(); // split filenames and only get the end value
                    String[] filename = fullfilename.Split("\\");
                    File.AppendAllText(iso2Directory + "\\FILES.TXT", "\n" + filename.Last());
                }

                var logFileIso1 = File.ReadAllLines(iso1Directory + "\\FILES.TXT");
                var log1List = new List<string>(logFileIso1);
                var logFileIso2 = File.ReadAllLines(iso2Directory + "\\FILES.TXT");
                var log2List = new List<string>(logFileIso2);

                // resolve case sensativity issues
                log1List = log1List.ConvertAll(d => d.ToLower());
                log2List = log2List.ConvertAll(d => d.ToLower());

                IEnumerable<String> removedFiles = log1List.Except(log2List);
                IEnumerable<String> addedFiles = log2List.Except(log1List);

                progressLabel.Text = "Comparing files...";
                foreach (String s in removedFiles)
                {
                    removedFilesRichTextBox.ReadOnly = false;
                    removedFilesRichTextBox.AppendText(s + "\n");
                    removedFilesRichTextBox.ReadOnly = true;
                }
                foreach (String s in addedFiles)
                {
                    addedFilesRichTextBox.ReadOnly = false;
                    addedFilesRichTextBox.AppendText(s + "\n");
                    addedFilesRichTextBox.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to compare files. This can be caused due to a corrupt INSTALL.WIM, or older INSTALL.WIM versions that aren't supported by DISM. Most builds of Windows Vista are currently not supported. The error was: " + ex.Message);
                // installwim1.Close();
                // installwim2.Close();
                DismApi.Shutdown();
            }
        }

        private bool validateIsos()
        {
            if (iso1FileNameTextBox.Text == string.Empty || iso2FileNameTextBox.Text == string.Empty)
            {
                abort();
                MessageBox.Show("Please enter an ISO file on both sides to compare.");
                return false;
            }
            if (iso1FileNameTextBox.Text == iso2FileNameTextBox.Text)
            {
                abort();
                MessageBox.Show("Cannot open the same file twice. Choose a different file for any of the two ISOs.");
                return false;
            }
            return true;
        }

        static void copyFile(DiscFileInfo file, string filePath)
        {
            using (FileStream fs = File.Create(filePath + "\\" + file.Name))
            {
                using (var fileStream = file.Open(FileMode.Open))
                {
                    fileStream.CopyTo(fs);
                }
            }
        }

        private void successMessage()
        {
            // this function allows deletion of temporary files if successful
            ISO1button.Enabled = true;
            ISO2button.Enabled = true;
            compareButton.Enabled = true;
            wikiTableButton.Enabled = true;
            exitButton.Enabled = true;

            // update progress
            progressLabel.Text = "Comparison complete";

            // make sure we run saveVersionInfo first before this
            // otherwise we won't be able to have access to the files
            // to fetch their version number and such
            System.IO.DirectoryInfo dir1 = new DirectoryInfo(iso1Directory);
            System.IO.DirectoryInfo dir2 = new DirectoryInfo(iso2Directory);
            String errortext = "Comparison has been completed. Do you want to delete the extracted temporary files?";
            String titletext = "Delete temporary files";
            Form2 confirmation = new Form2();
            confirmation.changeError(errortext, titletext);
            if (confirmation.ShowDialog() == DialogResult.Yes)
            {
                foreach (FileInfo file in dir1.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in dir1.EnumerateDirectories())
                {
                    dir.Delete(true);
                }
                foreach (FileInfo file in dir2.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in dir2.EnumerateDirectories())
                {
                    dir.Delete(true);
                }
                MessageBox.Show("Temporary files were successfully deleted.");
            }
        }

        private void wikiTableButton_Click(object sender, EventArgs e)
        {
            // check if the richtextboxes are empty
            // if so, abort

            if (addedFilesRichTextBox.Text == "" && removedFilesRichTextBox.Text == "")
            {
                MessageBox.Show("There's nothing to compare here. Either you didn't input two ISO files or these ISOs are identical in terms of the files it contains.");
                return;
            }

            // if not, we'll get from the removed and added files string list
            // and get them ready for a new form
            Form3 table = new Form3(addedFilesTableFormat, removedFilesTableFormat);
            table.ShowDialog();

            return;
        }

        private void saveVersionInfo(bool wim)
        {
            // let's put every file on each rich text box into their own string array
            // and put each line in there
            String[] removed = removedFilesRichTextBox.Lines;
            String[] added = addedFilesRichTextBox.Lines;

            // now, since we have a list of files in each pathway, we'll need to fetch their version number
            // and file description for everything we see
            // then we need to save it someplace
            // the easiest way to put these in a string list that is largely accessible to the rest of the program
            // so they can be saved even when we delete the temporary files later

            // format is "filename,description,fileversion"
            foreach (String s in removed)
            {
                if (!String.IsNullOrEmpty(s))
                {
                    if (!wim)
                    {
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(iso1Directory + "\\" + s);
                        removedFilesTableFormat.Add(s + "," + fileVersionInfo.FileDescription + "," + fileVersionInfo.FileVersion);
                    }
                    else
                    {
                        removedFilesTableFormat.Add(s + ",,");
                    }
                }
            }
            foreach (String s in added)
            {
                if (!String.IsNullOrEmpty(s))
                {
                    if (!wim)
                    {
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(iso2Directory + "\\" + s);
                        addedFilesTableFormat.Add(s + "," + fileVersionInfo.FileDescription + "," + fileVersionInfo.FileVersion);
                    }
                    else
                    {
                        addedFilesTableFormat.Add(s + ",,");
                    }
                }
            }
            // if we made it here, save was successful
            return;
        }

        private void extractCABs()
        {
            // this function does exactly what the function name does
            // note that this will also do the same with *.*_ files,
            // which are actually CAB files storing a single file
            // of the same name, hence why EXPAND.EXE would
            // work with them

            progressLabel.Text = "Extracting CAB files...";

            string[] iso1CABFiles = Directory.GetFiles(iso1Directory, "*.cab");
            string[] iso1filesendingwith_Files = Directory.GetFiles(iso1Directory, "*.*_");
            string[] iso2CABFiles = Directory.GetFiles(iso2Directory, "*.cab");
            string[] iso2filesendingwith_Files = Directory.GetFiles(iso2Directory, "*.*_");

            // i first did a loop to run an expand command; bad idea!
            // it'll try to open an expand command for each single file
            // which hogs memory

            // we could just run a single expand command for all cab files then
            // all *.*_ files
            // but if I knew more I could easily just try to extract the cab files
            // not many good choices though

            // only run when there's actually entries in the list so we don't waste time
            // these processes will run the next one automatically after 5 minutes in case they run too long or appears to hang
            extractProgressBar.Maximum = 100;
            extractProgressBar.Value = 0;
            if (iso1CABFiles.Any())
            {
                var process = Process.Start("expand.exe", "-R " + iso1Directory + "\\*.CAB");
                process.WaitForExit(300000);
            }
            extractProgressBar.Value = 25;
            if (iso1filesendingwith_Files.Any())
            {
                var process2 = Process.Start("expand.exe", "-R " + iso1Directory + "\\*.*_");
                process2.WaitForExit(300000);
            }
            extractProgressBar.Value = 50;
            if (iso2CABFiles.Any())
            {
                var process3 = Process.Start("expand.exe", "-R " + iso2Directory + "\\*.CAB");
                process3.WaitForExit(300000);
            }
            extractProgressBar.Value = 75;
            if (iso2filesendingwith_Files.Any())
            {
                var process4 = Process.Start("expand.exe", "-R " + iso2Directory + "\\*.*_");
                process4.WaitForExit(300000);
            }
            extractProgressBar.Value = 100;

            // delete the cab files
            foreach (string file in Directory.GetFiles(iso1Directory, "*.cab"))
            {
                File.Delete(file);
            }
            foreach (string file in Directory.GetFiles(iso1Directory, "*.*_"))
            {
                File.Delete(file);
            }
            foreach (string file in Directory.GetFiles(iso2Directory, "*.cab"))
            {
                File.Delete(file);
            }
            foreach (string file in Directory.GetFiles(iso2Directory, "*.*_"))
            {
                File.Delete(file);
            }
        }
    }
}