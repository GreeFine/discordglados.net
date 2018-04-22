using LibGit2Sharp;
using System;
using System.IO;

namespace DiscordBot.project_recover
{
    class RepoManager
    {
        static public string clone(string p_path, string name)
        {


            var co = new CloneOptions();
            co.CredentialsProvider = (_url, _user, _cred) =>
            new SshUserKeyCredentials
            {
                Username = "git",
                Passphrase = string.Empty,
                PublicKey = @"C:\Users\GreeFine\.ssh\id_rsa.pup",
                PrivateKey = @"C:\Users\GreeFine\.ssh\id_rsa"
            };
            Repository.Clone(p_path, @"./ClonedRepos/" + name, co);
            return "Done";
        }

        static public string cloneCmd(string p_git_path, string p_dir_path)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            Directory.CreateDirectory(p_dir_path);
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C cd " + p_dir_path + " & git clone \"" + p_git_path + "\"";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            return "Done";
        }

        static public string pullCmd(string p_projet_name, string p_dir_path)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            Directory.CreateDirectory(p_dir_path);
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C cd " + p_dir_path + "/" + p_projet_name + " & git pull";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            return "Done";
        }

        static public string cloneEpitech(string p_email, string p_projet_name)
        {
            string git_path = "git@git.epitech.eu:/" + p_email + "/" + p_projet_name;
            if (Directory.Exists(p_email + "/" + p_projet_name))
                pullCmd(p_projet_name, p_email);
            cloneCmd(git_path, p_email);
            return "Done";
        }
    }
}
