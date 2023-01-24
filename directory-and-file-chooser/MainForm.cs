using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Schema;

namespace directory_and_file_chooser
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            tsmiSaveAs.Click += onSaveAs;
            // Make sure there is a default value for the Folder
            if(string.IsNullOrWhiteSpace(Properties.Settings.Default.currentGunPath))
            {
                // POTENTIAL ISSUE - Ensure a valid default path
                Properties.Settings.Default.currentGunPath = Path.Combine
                (
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    typeof(MainForm).Assembly.GetName().Name
                );
            }
            // POTENTIAL ISSUE - Ensure directory exists.
            // (CreateDirectory will not destroy an existing directory.)
            Directory
                .CreateDirectory(Properties.Settings.Default.currentGunPath);
            gunSave.InitialDirectory = Properties.Settings.Default.currentGunPath;
        }
        
        private SaveFileDialog gunSave = new SaveFileDialog
        {
            Filter = "Json files | .json",
            Title = "Save the Gun",
        };
        private void onSaveAs(object? sender, EventArgs e)
        {            
            var text = textBoxRangedWeapon.Text;
            if(string.IsNullOrEmpty(text))
            {
                Debug.Assert(
                    false, 
                    "Expecting valid data. Don't enable the Save menu otherwise.");
                return;
            }
            gunSave.FileName = text;

            switch (gunSave.ShowDialog())
            {
                case DialogResult.OK:
                    break;
                case DialogResult.Cancel:
                    // The cancel condition needs to be handled.
                    return;
            }
            Properties.Settings.Default.currentGunPath =
                Path.GetDirectoryName(gunSave.FileName);
            // If settings are supposed to persist next time
            // the app runs, don't forget to Save.
            Properties.Settings.Default.Save();

            var savedgun = new Dictionary<string, object>()
            {
                {"Ranged Weapon", text },
            };
            var json = JsonConvert.SerializeObject(savedgun);

            // Now contains a fully qualified path.
            File.WriteAllText(gunSave.FileName, json);

#if DEBUG
            // Open the folder to view.
            Process.Start(
                "explorer.exe",
                Properties.Settings.Default.currentGunPath);
#endif
        }
    }
}