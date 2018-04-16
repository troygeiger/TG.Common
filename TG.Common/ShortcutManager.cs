using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Common
{
    public static class ShortcutManager
    {
        public static bool CreateShortcut(string path, string target, string arguments, string description)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = shell.CreateShortcut(path) as IWshShortcut;

                shortcut.TargetPath = target;
                shortcut.Description = description;
                shortcut.Arguments = arguments;
                shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(target);

                shortcut.Save();
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public static bool CreateShortcut(string path, string target, string arguments, string description, string iconPath)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = shell.CreateShortcut(path) as IWshShortcut;

                shortcut.TargetPath = target;
                shortcut.Description = description;
                shortcut.Arguments = arguments;
                shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(target);
                shortcut.IconLocation = iconPath;

                shortcut.Save();
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public static bool CreateShortcut(string path, string target, string arguments, string description, string workingDirectory, string iconPath)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = shell.CreateShortcut(path) as IWshShortcut;

                shortcut.TargetPath = target;
                shortcut.Description = description;
                shortcut.Arguments = arguments;
                shortcut.WorkingDirectory = workingDirectory;
                shortcut.IconLocation = iconPath;

                shortcut.Save();
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }
    }
}
