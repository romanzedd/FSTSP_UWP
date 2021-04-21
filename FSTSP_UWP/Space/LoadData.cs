using System;
//using System.Windows.Forms;

namespace FSTSP_UWP
{
    class LoadData
    {
        public void loadNodes(string file, int numLines, ref Graph[] node)
        {
            string[] lines = file.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );

            for (int i = 0; i < numLines - 1; i++)
            {
                double altitude = 20.0;
                string[] words = lines[i].Split('\t');
                node[i] = new Graph(i + 1, words[1], words[2], words[3], double.Parse(words[4], System.Globalization.CultureInfo.InvariantCulture),
                    double.Parse(words[5], System.Globalization.CultureInfo.InvariantCulture), altitude, 0);
                altitude += 30;
                node[i + (numLines - 1) * 1] = new Graph(i + (node.Length / 4 * 1) + 1, words[1], words[2], words[3], double.Parse(words[4], System.Globalization.CultureInfo.InvariantCulture),
                    double.Parse(words[5], System.Globalization.CultureInfo.InvariantCulture), altitude, 1);
                altitude += 50;
                node[i + (numLines - 1) * 2] = new Graph(i + (node.Length / 4 * 2) + 1, words[1], words[2], words[3], double.Parse(words[4], System.Globalization.CultureInfo.InvariantCulture),
                    double.Parse(words[5], System.Globalization.CultureInfo.InvariantCulture), altitude, 2);
                altitude += 50;
                node[i + (numLines - 1) * 3] = new Graph(i + (node.Length / 4 * 3) + 1, words[1], words[2], words[3], double.Parse(words[4], System.Globalization.CultureInfo.InvariantCulture),
                    double.Parse(words[5], System.Globalization.CultureInfo.InvariantCulture), altitude, 3);
            }
            return;
        }
        public void loadRoutes(string file, ref Graph[] node)
        {
            string[] lines = file.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );

            int linesNum = lines.Length - 1;
            for (int i = 0; i < linesNum; i++)
            {
                string[] words = lines[i].Split('\t');
                int lineLength = 0;
                for (int j = 0; j < words.Length; j++)
                {
                    if (words[j] != "")
                        lineLength++;
                }
                int[] indexes = new int[lineLength];
                for (int j = 0; j < lineLength; j++)
                {
                    Int32.TryParse(words[j], out indexes[j]);
                }

                for (int j = 1; j < lineLength; j++)
                {
                    Rib edge = new Rib();
                    routing route = new routing();
                    node[indexes[0] - 1].neighbourhood.Add(node[indexes[j] - 1]);

                    edge.ConnectedNode = node[indexes[j] - 1];
                    edge.Length = route.Haversine(node[indexes[0] - 1].latitude, node[indexes[j] - 1].latitude, node[indexes[0] - 1].longitude, node[indexes[j] - 1].longitude, node[indexes[0] - 1].altitude, node[indexes[j] - 1].altitude);
                    node[indexes[0] - 1].connections.Add(edge);
                    //node[indexes[0] - 1 + node.Length / 4].neighbourhood.Add(node[indexes[j] - 1 + node.Length / 4]); //работает правильно, на на уровне 50 метров в данном квратале нет препятствий
                    //node[indexes[0] - 1 + node.Length / 4 * 2].neighbourhood.Add(node[indexes[j] - 1 + node.Length / 4 * 2]);
                    //node[indexes[0] - 1 + node.Length / 4 * 3].neighbourhood.Add(node[indexes[j] - 1 + node.Length / 4 * 3]);
                }
            }

            for (int i = 0; i < node.Length / 4; i++)
            {
                for (int j = 0; j < node.Length / 4; j++)
                {
                    Rib edge = new Rib();
                    routing route = new routing();
                    if (j == i && j < (node.Length / 4 - 1))
                        j++;
                    node[i + (node.Length / 4)].neighbourhood.Add(node[j + (node.Length / 4)]);
                    node[i + (node.Length / 4) * 2].neighbourhood.Add(node[j + (node.Length / 4) * 2]);
                    node[i + (node.Length / 4) * 3].neighbourhood.Add(node[j + (node.Length / 4) * 3]);

                    edge.ConnectedNode = node[j + (node.Length / 4)];
                    edge.Length = route.Haversine(node[i + (node.Length / 4)].latitude, node[j + (node.Length / 4)].latitude, node[i + (node.Length / 4)].longitude, node[j + (node.Length / 4)].longitude, node[i + (node.Length / 4)].altitude, node[j + (node.Length / 4)].altitude);
                    node[i + (node.Length / 4)].connections.Add(edge);

                    edge.ConnectedNode = node[j + (node.Length / 4) * 2];
                    edge.Length = route.Haversine(node[i + (node.Length / 4) * 2].latitude, node[j + (node.Length / 4 * 2)].latitude, node[i + (node.Length / 4 * 2)].longitude, node[j + (node.Length / 4 * 2)].longitude, node[i + (node.Length / 4 * 2)].altitude, node[j + (node.Length / 4 * 2)].altitude);
                    node[i + (node.Length / 4 * 2)].connections.Add(edge);

                    edge.ConnectedNode = node[j + (node.Length / 4) * 3];
                    edge.Length = route.Haversine(node[i + (node.Length / 4) * 3].latitude, node[j + (node.Length / 4 * 3)].latitude, node[i + (node.Length / 4 * 3)].longitude, node[j + (node.Length / 4 * 3)].longitude, node[i + (node.Length / 4 * 3)].altitude, node[j + (node.Length / 4 * 3)].altitude);
                    node[i + (node.Length / 4 * 3)].connections.Add(edge);
                }
            }
            for (int i = 0; i < (node.Length / 4); i++)
            {
                Rib edge = new Rib();
                routing route = new routing();

                node[i].neighbourhood.Add(node[i + (node.Length / 4)]);

                node[i + (node.Length / 4)].neighbourhood.Add(node[i]);
                node[i + (node.Length / 4)].neighbourhood.Add(node[i + (node.Length / 4) * 2]);

                node[i + (node.Length / 4) * 2].neighbourhood.Add(node[i + (node.Length / 4) * 3]);
                node[i + (node.Length / 4) * 2].neighbourhood.Add(node[i + (node.Length / 4)]);

                node[i + (node.Length / 4) * 3].neighbourhood.Add(node[i + (node.Length / 4) * 2]);
                //0
                edge.ConnectedNode = node[i + (node.Length / 4)];
                edge.Length = route.Haversine(node[i].latitude, node[i + (node.Length / 4)].latitude, node[i].longitude, node[i + (node.Length / 4)].longitude, node[i].altitude, node[i + (node.Length / 4)].altitude);
                node[i].connections.Add(edge);

                //1
                edge.ConnectedNode = node[i];
                edge.Length = route.Haversine(node[i].latitude, node[i + (node.Length / 4)].latitude, node[i].longitude, node[i + (node.Length / 4)].longitude, node[i].altitude, node[i + (node.Length / 4)].altitude);
                node[i + (node.Length / 4)].connections.Add(edge);

                edge.ConnectedNode = node[+(node.Length / 4) * 2];
                edge.Length = route.Haversine(node[i + (node.Length / 4)].latitude, node[i + (node.Length / 4) * 2].latitude, node[i + (node.Length / 4)].longitude, node[i + (node.Length / 4) * 2].longitude, node[i + (node.Length / 4)].altitude, node[i + (node.Length / 4) * 2].altitude);
                node[i + (node.Length / 4)].connections.Add(edge);

                //2
                edge.ConnectedNode = node[+(node.Length / 4) * 3];
                edge.Length = route.Haversine(node[i + (node.Length / 4) * 2].latitude, node[i + (node.Length / 4) * 3].latitude, node[i + (node.Length / 4) * 2].longitude, node[i + (node.Length / 4) * 3].longitude, node[i + (node.Length / 4) * 2].altitude, node[i + (node.Length / 4) * 3].altitude);
                node[i + (node.Length / 4) * 2].connections.Add(edge);

                edge.ConnectedNode = node[+(node.Length / 4)];
                edge.Length = route.Haversine(node[i + (node.Length / 4) * 2].latitude, node[i + (node.Length / 4)].latitude, node[i + (node.Length / 4) * 2].longitude, node[i + (node.Length / 4)].longitude, node[i + (node.Length / 4) * 2].altitude, node[i + (node.Length / 4)].altitude);
                node[i + (node.Length / 4) * 2].connections.Add(edge);

                //3
                edge.ConnectedNode = node[+(node.Length / 4) * 2];
                edge.Length = route.Haversine(node[i + (node.Length / 4) * 2].latitude, node[i + (node.Length / 4) * 3].latitude, node[i + (node.Length / 4) * 2].longitude, node[i + (node.Length / 4) * 3].longitude, node[i + (node.Length / 4) * 2].altitude, node[i + (node.Length / 4) * 3].altitude);
                node[i + (node.Length / 4) * 3].connections.Add(edge);
            }
        }

        //public int fileDecrypt(string filename, ref string file)
        //{
        //    file = AES_Decrypt(filename, Encoding.UTF8.GetBytes("password"));
        //    int numLines = file.Split('\n').Length;
        //    return numLines;
        //}
        //private static string openFileDlg()//commented out for UWP application. If needed fix
        //{
        //    Stream myStream = null;
        //    OpenFileDialog openFileDialog1 = new OpenFileDialog();

        //    openFileDialog1.InitialDirectory = "c:\\";
        //    openFileDialog1.Filter = "map files (*.map)|*.map";
        //    openFileDialog1.FilterIndex = 2;
        //    openFileDialog1.RestoreDirectory = true;

        //    if (openFileDialog1.ShowDialog() == DialogResult.OK)
        //    {
        //        try
        //        {
        //            if ((myStream = openFileDialog1.OpenFile()) != null)
        //            {
        //                using (myStream) { }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
        //        }
        //    }
        //    else
        //        return null;
        //    return openFileDialog1.FileName;
        //}
        //private static string AES_Decrypt(string inputFile, byte[] passwordBytes)
        //{            
        //    byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        //    if (!File.Exists(inputFile))
        //    {
        //        MessageBox.Show("Error: Could not find *.map file");
        //        if ((inputFile = openFileDlg()) == null)
        //        {
        //            MessageBox.Show("Unable to open the *.map file. Application is shutting down");
        //            Application.Exit();
        //        }
        //    }         

        //    FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

        //    RijndaelManaged AES = new RijndaelManaged();

        //    AES.KeySize = 256;
        //    AES.BlockSize = 128;

        //    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
        //    AES.Key = key.GetBytes(AES.KeySize / 8);
        //    AES.IV = key.GetBytes(AES.BlockSize / 8);
        //    AES.Padding = PaddingMode.Zeros;

        //    AES.Mode = CipherMode.CBC;

        //    CryptoStream cs = new CryptoStream(fsCrypt,
        //        AES.CreateDecryptor(),
        //        CryptoStreamMode.Read);
        //    StreamReader reader = new StreamReader(cs);
        //    return reader.ReadToEnd();
        //}
    }
}
