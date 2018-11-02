using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

using System.Drawing;

using System.IO;

namespace InputImagesToSSMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SqlConnection DBcnn;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // connect to DB
            DB_connection_start();

            /*
             * INSERT INTO Images_door(idImage, Image_saved) 
                SELECT 0, BulkColumn 
                FROM Openrowset( Bulk 'C:\Users\Jonathan\Documents\Visual Studio 2013\Projects\Client_MVP\Client_MVP\images\closed_door.png', Single_Blob) as Image_saved

                INSERT INTO Images_door(idImage, Image_saved) 
                SELECT 0, BulkColumn 
                FROM Openrowset( Bulk 'C:\Users\Jonathan\Documents\Visual Studio 2013\Projects\Client_MVP\Client_MVP\images\open_door.png', Single_Blob) as Image_saved


            */

            //Uri imageUri = new Uri("C:\\Users\\Jonathan\\Documents\\Visual Studio 2013\\Projects\\Client_MVP\\Client_MVP\\images\\closed_door.png", UriKind.Absolute);
            //BitmapImage imageBitmap = new BitmapImage(imageUri);
            
            //image to byteArray
            //byte[] bArr = BufferFromImage(imageBitmap);
            //byte[] bArr = imgToByteConverter(img);
            //Again convert byteArray to image and displayed in a picturebox
            //Image img1 = byteArrayToImage(bArr);

            string open_door_path = "C:\\Users\\Jonathan\\Documents\\Visual Studio 2013\\Projects\\Client_MVP\\Client_MVP\\images\\closed_door.png";

            byte[] b = File.ReadAllBytes(open_door_path);
            //MessageBox.Show(b.Length.ToString());

            // --string converted = bTOs_Convert(b);
            //MessageBox.Show(converted);

            
            string sql_closed = "INSERT INTO Images_door ( image_name, image_saved) VALUES ('closed_door', @byteArr);";
            using (SqlCommand cmd1 = new SqlCommand(sql_closed, DBcnn))
            {
                cmd1.Parameters.Add("@byteArr", System.Data.SqlDbType.VarBinary, -1).Value = b; // BIG ENOUGH SIZE IS IMPORTANT

                cmd1.ExecuteNonQuery();  

            } // command disposed here
            
            
            // Retrieve and show
            /*
            string table = "Select * FROM images_door;";
            using (SqlCommand cmdSelect = new SqlCommand(table, DBcnn))
            {
                using (SqlDataReader reader1 = cmdSelect.ExecuteReader())
                {
                    if (reader1 != null)
                    {
                        while (reader1.Read())
                        {
                            //MessageBox.Show((string)reader1.GetValue(1));
                            // pull string of the byte array from DB
                            //string s_img = (string)reader1.GetString(2);
                            //MessageBox.Show("55: " + s_img);

                            // convert back to byte array and show image
                            //byte[] con = sTOb_Convert(s_img);
                            byte[] con = (byte[])reader1.GetValue(2);

                            // convert back to image from byte array
                            //BitmapImage bi = ImageFromBuffer(con);
                            //img_show.Source = mybmp;
                            //byte[] bb = sTOb_Convert();

                            MemoryStream stream = new MemoryStream(con);
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.StreamSource = stream;
                            image.EndInit();

                            img_show.Source = image;
                            // --img_floor.Source = bi;
                            //MessageBox.Show("stop here");
                            break;
                            /*
                            // file name and extension
                            string save_file = current_dir + folder + "\\" + reader1.GetValue(1) + ".jpg";

                            // save image to file
                            FileStream fs = new FileStream(save_file, FileMode.CreateNew, FileAccess.ReadWrite);
                            fs.Write(img_bytes, 0, img_bytes.Length);
                            fs.Flush();
                            fs.Close();

                           */
                  //      }
                //    }
              //  }
            //} 
            
            //
          


            
            string open_door_path1 = "C:\\Users\\Jonathan\\Documents\\Visual Studio 2013\\Projects\\Client_MVP\\Client_MVP\\images\\open_door.png";

            byte[] b1 = File.ReadAllBytes(open_door_path1);
            string converted1 = bTOs_Convert(b1);
            //string converted = Encoding.UTF8.GetString(b, 0, b.Length);
            //MessageBox.Show(converted);

            string sql_closed1 = "INSERT INTO Images_door (image_name, image_saved) VALUES ('open_door', @byteArr);";
            using (SqlCommand cmd2 = new SqlCommand(sql_closed1, DBcnn))
            {
                cmd2.Parameters.Add("@byteArr", System.Data.SqlDbType.VarBinary, -1).Value = b1;

                cmd2.ExecuteNonQuery();

            } // command disposed here

            // convert back to image from byte array
            //BitmapImage bi = ImageFromBuffer(b);
            //img_show.Source = bi;

            // =======================

            // hard coded update for floor plans


            string open_door_path2 = "C:\\Users\\Jonathan\\Documents\\Visual Studio 2013\\Projects\\Client_MVP\\Client_MVP\\images\\main_floor.jpg";

            byte[] b2 = File.ReadAllBytes(open_door_path2);

            string converted2 = bTOs_Convert(b2);
            //string converted = Encoding.UTF8.GetString(b, 0, b.Length);
            //MessageBox.Show(converted);

            string sql_closed2 = "UPDATE floor_collection_0 SET Image_floor = @flr_plan WHERE idFloor = 0;";
            using (SqlCommand cmd3 = new SqlCommand(sql_closed2, DBcnn))
            {
                cmd3.Parameters.Add("@flr_plan", System.Data.SqlDbType.VarBinary, -1).Value = b2; // -1 is for max (> 8000 bytes)

                cmd3.ExecuteNonQuery();

            } // command disposed here


            string open_door_path3 = "C:\\Users\\Jonathan\\Documents\\Visual Studio 2013\\Projects\\Client_MVP\\Client_MVP\\images\\Floor_replace.jpg";

            byte[] b3 = File.ReadAllBytes(open_door_path3);
            string converted3 = bTOs_Convert(b3);
            //string converted = Encoding.UTF8.GetString(b, 0, b.Length);
            //MessageBox.Show(converted);

            string sql_closed3 = "UPDATE floor_collection_0 SET Image_floor = @flr_plan WHERE idFloor = 1;";
            using (SqlCommand cmd4 = new SqlCommand(sql_closed3, DBcnn))
            {
                cmd4.Parameters.Add("@flr_plan", System.Data.SqlDbType.VarBinary, -1).Value = b3;

                cmd4.ExecuteNonQuery();

            } // command disposed here
            
            MessageBox.Show("SUCCESS!!???");

            // =======================

            // end connection
            DB_connection_end();
        }

        public byte[] sTOb_Convert(string s)
        {
            byte[] characters = s.ToCharArray().Select(c => (byte)c).ToArray();
            return characters;
        }

        public string bTOs_Convert(byte[] data)
        {
            char[] characters = data.Select(b => (char)b).ToArray();
            return new string(characters);
        }

        public BitmapImage ImageFromBuffer(Byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        private int DB_connection_start()
        {
            string connectionString;


            // Set connection string, CHANGE WHEN DB CREATED
            // ServerName, DatabaseName
            connectionString = "server=(local);user id=sa;database=floor_collection;Password=Pa55w0rd?";

            // assign connection
            DBcnn = new SqlConnection(connectionString);

            // Open connection
            // check is no current connection
            if (DBcnn != null && DBcnn.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    DBcnn.Open();
                }
                catch (InvalidOperationException e)
                {
                    //set_DB_error(e.ToString());
                    return 1; // cannot open connection OR connection is already open
                }
                catch (SqlException e)
                {
                    //set_DB_error(e.ToString());
                    return 1; // 
                }
            }


            return 0;
        }

        public string Convert(byte[] data)
        {
            char[] characters = data.Select(b => (char)b).ToArray();
            return new string(characters);
        }

        // try to end connection to DB server
        private int DB_connection_end()
        {
            if (DBcnn != null && DBcnn.State == System.Data.ConnectionState.Closed)
            {
                // no connection to close
                return 0;
            }
            else
            {
                DBcnn.Close();
                return 0;
            }

            //return 0;
        }
    }
}
