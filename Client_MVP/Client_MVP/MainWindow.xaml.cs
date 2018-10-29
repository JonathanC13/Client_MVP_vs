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
using System.IO;
// Would I want to pull every time the user switches floors 
// OR 
// store instance for a set amount of time.

// What I want to pull from the DB
/*
 *  1. Floor names, images, and number of floors (maybe just count)
 *  2. Doors for that floor: Name, location. From name I can create the button and on_click function
 *  3. Door time that it will remain open when unlocked
 *  4. 
 */

/*
 * Handling ===
 * From floor pull, order them in an array based on column "floor order." Populate combo box and link the path of the downloaded floor plan to the click
 *  `- When ordering also, fill the jagged array meant for the doors on that floor
 *  
 * Operation ===
 * On combo box selection, the image is loaded into the scrollviewer and the doors are placed based on the margin values
 */ 

namespace Client_MVP
{
    // struct for the door's location and use for floor name
        public struct door_struct
        {
            public string name; //Door_1

            // Button attributes
            public double[] door_margin; // margin L,T,R,B - Default HorizontalAlignment = Left, VerticalAlignment = Top
                                                // Default set Height ="50" Width="50" Background="Red" FontSize="14" Foreground="White"
                                                // Click="btn_Door_1_Click" created based on name
            // Button IP
            public string door_IP;

        }

        public struct floor_struct
        {
            public string og_flr_name; // original name

            public int floor_num; // floor number

            // floor name
            public string name; // display name

            // Floor attributes
            public string floor_image; // Exclusively for floor table, get the path to the downloaded image.
            
            public int door_ID; // door ID to know which doors in the door table that are relevent
        }

        public class Flr_Dr
        {
            public floor_struct st_floor;
            public List<door_struct> arr_doors = new List<door_struct>();

            public Flr_Dr()
            {
                arr_doors = new List<door_struct>();
            }

            // <Getters/Setters>
            // Get display name
            public string getDisplayName()
            {
                return st_floor.name;
            }

            public string getFlrName()
            {
                return st_floor.og_flr_name;
            }

            // Set floor_struct
            public void setFloor_st(string floor_name, int floor_number, string display_name, string img_dir, int doorID)
            {
                st_floor = new floor_struct() { og_flr_name = floor_name, floor_num = floor_number, name = display_name, floor_image = img_dir, door_ID = doorID };
            }

            public int getDoorID()
            {
                return st_floor.door_ID;
            }

            // </Getters/Setters>

            // <List modify>
            // Create door_struct and Add to List
            public void addDoor(string drName, double[] dr_margin, string drIP)
            {
                door_struct curr_door = new door_struct() { name = drName, door_margin = dr_margin, door_IP = drIP};
                arr_doors.Add(curr_door);
            }

            // </List modify>
        }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct button_struct
        {
            public Button btn_door;
            public string IP_door;
        }

        // Used to store a class which contains the floor and a List of the doors on that floor.
        public List<Flr_Dr> flr_dr_class_lst = new List<Flr_Dr>();

        // ordered list for the floor structs, 0 as the lowest floor
        //private List<floor_struct> arr_floor = new List<floor_struct>();
        
        // pure string array to set the combobox items
        private List<string> arr_flr_name = new List<string>();       

        // Jagged array (array of arrays). Each index represents the floor number and the array it contains is the doors on that floor
        //private List<List<door_struct>> jagged_doors = new List<List<door_struct>>();
        //private door_struct[][] jagged_doors; 
        /*
         * int[][] jaggedArray = new int[3][];
         * 
         * jaggedArray[0] = new int[5];
         * jaggedArray[1] = new int[4];
         * jaggedArray[2] = new int[2];
         * 
         * jaggedArray[0] = new int[] { 1, 3, 5, 7, 9 };
         * jaggedArray[1] = new int[] { 0, 2, 4, 6 };
         * jaggedArray[2] = new int[] { 11, 22 };
         * 
         * 
         */ 

        // Time for door to remain unlocked for user
        private int time_door_open;

        // List for placed doors
        private List<button_struct> placed_doors = new List<button_struct>();

        // SQL connection
        private SqlConnection DBcnn;

        // SQL command
        SqlCommand command; // NOTE: Can't have nested SqlCommands open.        
        private string flr_table = "floor_collection_0";
        private string dr_table = "door_collection_0";

        // DB recent error message
        private string DB_error = "";

        // directory to save images
        private string folder = "\\images";
        private string current_dir = System.Environment.CurrentDirectory;

        public MainWindow()
        {
            InitializeComponent();

            int success = 1;
            for (int i = 0; i < 3; i++)
            {
                // Attempt to connect to DB server. Maybe have loop to try 3 times before quit
                int status_connect_start = DB_connection_start();
                if (status_connect_start == 0)
                {
                    success = 0;
                    break;
                }
            }

            if (success == 0)
            {
                // Download and store floor images. Also pull images for closed and open door icons
                dl_str_floor_img();
                
                // define the 2d array for the floors and its doors
                DB_pull_floors();   

                // Fill combo box with the floors
                set_combobox_items();

                // define time for door to remain open
                DB_pull_time_door_open();

                // End connection if established in the first place
                int status_connect_end = DB_connection_end();

            }
            else
            {
                string pop_error = "Cannot connect to Database: " + get_DB_message();
                MessageBox.Show(pop_error);
            }

            //MessageBox.Show("Intialized");
        }

        // try to connect to DB server
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
                    set_DB_error(e.ToString());
                    return 1; // cannot open connection OR connection is already open
                }
                catch (SqlException e)
                {
                    set_DB_error(e.ToString());
                    return 1; // 
                }
            }
            

            return 0;
        }

        private void set_DB_error(string err_msg)
        {
            DB_error += err_msg;
        }

        private string get_DB_message()
        {
            return DB_error;
        }

        
        // Used by other functions to execute their SQL commands to the DB. Cant use
        // Input: input_command: string, sql statement to be executed
        // Output: output:string, the data read from the table.
        private SqlDataReader execute_SQL(string input_command)
        {
            
            SqlDataReader dataReader;
            String sql;

            // Define sql statement
            sql = input_command;

            // Command statement. sql command and DB connection
            command = new SqlCommand(sql, DBcnn);

            // Define the data reader
            dataReader = command.ExecuteReader();

            return dataReader;

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

        // fill arr_floor ordered from lowest floor to highest and also filling the jagged_door array with the doors on the specific floor.
        private void DB_pull_floors()
        {

            // cannot parameterize table name
            String select_floor = "SELECT * FROM " + flr_table + ";";
            // May have DB floors ordered based on column "floor order", but still check

            // count number of rows
            int floor_rows = 0;
            using (SqlCommand cmd = new SqlCommand(select_floor, DBcnn))
            {
                cmd.Parameters.Add("@floor_table", System.Data.SqlDbType.VarChar).Value = flr_table;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            //MessageBox.Show("row: " + (dataReader.GetDecimal(2)).ToString());
                            floor_rows++;
                        }
                    }
                } // reader closed and disposed up here

            } // command disposed here
            //MessageBox.Show("1. floors looped: " + floor_rows.ToString());

            int floor_count = 0;            
            
            // Get the table values
            while (floor_count < floor_rows) // while true
            {
                
                // need new instance of reader to start at first row
                using (SqlCommand cmd = new SqlCommand(select_floor, DBcnn))
                {
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                //MessageBox.Show("looking: " + (double)reader.GetDecimal(2));
                                // Parent Table columns, Floor Table
                                // Floor ID | Name | Floor order | Image Path | DoorID
                                // add in order, just in case out of order
                                // Admin app will sort  DB after finalizing, so this is just to make sure
                                if ((int)reader.GetValue(2) == floor_count)
                                {
                                    // Create class object
                                    Flr_Dr new_flr = new Flr_Dr();

                                    string floor_name = (reader.GetValue(1)).ToString();
                                    string flr_numbered = floor_count + ". " + floor_name; // display name
                                    string img_dir = folder + floor_name + ".jpg";                                    
                                    int doorID = ((int)reader.GetValue(4));
                                    
                                    // create floor struct in new_flr
                                    new_flr.setFloor_st(floor_name, floor_count, flr_numbered, img_dir, doorID);

                                    // Add new_flr to class list
                                    flr_dr_class_lst.Add(new_flr);
                                    
                                    // Don't break, may be more floorplans for the same floor. Floors with the same FloorOrder value
                                }
                            }
                            // 
                            floor_count++;
                        }
                    } // reader closed and disposed up here

                } // command disposed here

                //MessageBox.Show("in loop: " + floor_count.ToString());
            }

            //MessageBox.Show("Last loop " +floor_number.ToString());

            foreach (Flr_Dr flr in flr_dr_class_lst)
            {
                
                // Fill doorLists based on DoorID from floor_struct
                int pull_dr_ID = flr.getDoorID();
                fill_door_arr(flr, pull_dr_ID);
            }
            
        }

        // fill the door array for the specified floor
        // Door table is a child of floor table and the foreign key is Floor group ID
        private void fill_door_arr(Flr_Dr cur_flr, int IN_door_id)
        {

            String select_door = "SELECT * FROM " + dr_table + ";";            

            using (SqlCommand cmd1 = new SqlCommand(select_door, DBcnn))
            {
                
                using (SqlDataReader reader1 = cmd1.ExecuteReader())
                {
                    if (reader1 != null)
                    {
                        while (reader1.Read())
                        {

                            // Door table
                            // Door ID | Door Name | Floor order (Foreign key) | Margin Left | Margin Top | Margin Right | Margin Bottom | Door IP ;; if same floor 1.0, 1.1, 1.2 etc
                            if ((int)reader1.GetValue(2) == IN_door_id)
                            {
                                double[] door_margin_curr = new double[4]; // Must create new instance of this, so during the list.add it does not overwrite the previous value

                                // append margins                 
                                door_margin_curr[0] = (double)reader1.GetDecimal(3);
                                door_margin_curr[1] = (double)reader1.GetDecimal(4);
                                door_margin_curr[2] = (double)reader1.GetDecimal(5);
                                door_margin_curr[3] = (double)reader1.GetDecimal(6);

                                string dr_name = (string)reader1.GetValue(1);
                                string dr_IP = (string)reader1.GetValue(7);

                                // create and add door to door list for this floor
                                cur_flr.addDoor(dr_name, door_margin_curr,dr_IP);                                
                            }
                        }
                    }
                    
                } // reader closed and disposed up here   

            } // command disposed here   

        } //connection closed and disposed here if connection was set up

        // Download and store floorplan images in a specific directory
        // Input: null
        // Output: null
        private void dl_str_floor_img()
        {
            // Check if folder exists, if not create.
            // The path parameter specifies a directory path, not a file path. If the directory already exists, this method does nothing.
            string full_dir = current_dir + folder;
            try
            {
                //MessageBox.Show(full_dir);
                // Determine whether the directory exists.
                if (Directory.Exists(full_dir))
                {
                    // if exist, do nothing
                }

                // Try to create the directory. If exist, this doesnt do anything
                DirectoryInfo di = Directory.CreateDirectory(full_dir);
                //MessageBox.Show("The directory was created successfully at {0}." + Directory.GetCreationTime(full_dir));

                // Delete the directory.
                //di.Delete();
                
            }
            catch (Exception e)
            {
                //MessageBox.Show("111 The process failed: {0}", e.ToString());
            }
            finally { }

            // Pull floor images
            string flr_0 = "floor_collection_0"; // table name
            // pull image from DB and save it
            rtv_image(flr_0, 3); // table_name, col that contains the string for the image

            // Pull door images from DB
            string dr_img_0 = "Images_door"; // table name
            // pull image from DB and save it
            rtv_image(dr_img_0, 2);
        }

        // Retrieve image and save it
        // Input: table_name: string, table name; image_col: int; col that contains the string for the image
        // Output: null
        private void rtv_image(string table_name, int image_col)
        {
           string table = "select * FROM " + table_name + ";";
            try
            {

                using (SqlCommand cmdSelect = new SqlCommand(table, DBcnn))
                {
                    using (SqlDataReader reader1 = cmdSelect.ExecuteReader())
                    {
                        if (reader1 != null)
                        {
                            while (reader1.Read())
                            {
                                
                                // pull byte array from DB
                                byte[] b_img = (byte[])reader1.GetValue(image_col); 
                               
                                // convert back to image from byte array
                                //BitmapImage bi = ImageFromBuffer(b_img);
                                //img_floor.Source = bi;
                               

                                // --img_floor.Source = bi;
                                //MessageBox.Show("stop here");
                                
                                // file name and extension
                                string save_file = current_dir + folder + "\\" + reader1.GetValue(1) + ".jpg";

                                if (File.Exists(save_file))
                                {
                                    try
                                    {
                                        File.Delete(save_file);
                                    }
                                    catch(Exception ex)
                                    {
                                        //MessageBox.Show("delete: " + ex.ToString());
                                    }
                                }

                                // save image to file
                                FileStream fs = new FileStream(save_file, FileMode.CreateNew, FileAccess.ReadWrite);
                                fs.Write(b_img, 0, b_img.Length);
                                fs.Flush();
                                fs.Close();

                               
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("333" + ex.Message);
            }

        }

        public byte[] sTOb_Convert(string s)
        {
            byte[] characters = s.ToCharArray().Select(c => (byte)c).ToArray();
            return characters;
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

        private void set_combobox_items()
        {   
            /*
            ComboBoxItem comboBoxItem1 = new ComboBoxItem();
            comboBoxItem1.Content = "Current Floor";
            comboBoxItem1.IsSelected = true;

            cmb_floor_list.Items.Add(comboBoxItem1);
            */
            arr_flr_name = new List<string>();

            // fill an array with only the floor name, assuming already ordered from lowest to highest level
            foreach (Flr_Dr flr in flr_dr_class_lst)
            {
                arr_flr_name.Add(flr.getDisplayName());
            }

            // set combo box to the floor array
            cmb_floor_list.ItemsSource = arr_flr_name;
            cmb_floor_list.SelectedIndex = 0;
            
            // Initial floor plan loaded into scrollviewer
            Dispatcher.Invoke(() =>
                {
                    // floor name at top
                    lbl_floor_name.Text = flr_dr_class_lst[0].getDisplayName();

                    string open_door_path = current_dir + folder + "\\" + flr_dr_class_lst[0].getFlrName() + ".jpg";
                    try
                    {
                        open_door_path = System.IO.Path.GetFullPath(open_door_path);
                        //MessageBox.Show("path: " + open_door_path);
                    }
                    catch (ArgumentException ex)
                    {
                        //MessageBox.Show(open_door_path + ": " + ex.ToString());
                    }
                    //Uri imageUri = new Uri(open_door_path, UriKind.Relative);
                    //BitmapImage imageBitmap = new BitmapImage(imageUri);
                    //Image myImage = new Image();
                    //myImage.Source = imageBitmap;

                    //img_floor.Source = myImage.Source;
                    byte[] sel_img = File.ReadAllBytes(open_door_path);
                    BitmapImage sel_bb = ImageFromBuffer(sel_img);

                    img_floor.Source = sel_bb;

                
                }
            );
            clear_prev_doors(); // Clearing before adding initial doors helps it clear when another combo box item is selected
            // place doors on the floor plan
            place_curr_doors(0);
        }

        // Pull time for door to remain open
        private void DB_pull_time_door_open()
        {
            time_door_open = 3000;
        }

        // when a combobox item is selected
        private void cmb_floor_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

            // items are named with the convention" 1. First floor / 2. Second floor / etc
            int floor_picked = cmb_floor_list.SelectedIndex;

            // once the floor has been determined, load the corresponding floor plan into the scroll viewer
            Dispatcher.Invoke(() =>
                {
                    // change title of floor
                    lbl_floor_name.Text = flr_dr_class_lst[floor_picked].getDisplayName();

                    // Change the image
                    // Set the images associated with each floor in the combo box

                    string open_door_path = current_dir + folder + "\\" + flr_dr_class_lst[floor_picked].getFlrName() + ".jpg";
                    //string open_door_path = arr_floor[floor_picked].floor_image;
                    try
                    {
                        open_door_path = System.IO.Path.GetFullPath(open_door_path);
                        //MessageBox.Show("path: " + open_door_path);
                    }
                    catch (ArgumentException ex)
                    {
                        //MessageBox.Show(open_door_path + ": " + ex.ToString());
                    }

                    //Uri imageUri = new Uri(open_door_path, UriKind.Relative);   // relative after all works
                    //BitmapImage imageBitmap = new BitmapImage(imageUri);
                    //Image myImage = new Image();
                    //myImage.Source = imageBitmap;

                    //img_floor.Source = myImage.Source;

                    byte[] sel_img = File.ReadAllBytes(open_door_path);
                    BitmapImage sel_bb = ImageFromBuffer(sel_img);

                    img_floor.Source = sel_bb;


                    // Also need to clear previous doors on the floorplan
                    clear_prev_doors();

                    // After cleared
                    // Place doors for current floor
                    place_curr_doors(floor_picked);
                }
            );
        }

        private void clear_prev_doors()
        {
            foreach (button_struct btn in placed_doors)
            {
                
                grd_ScrollV.Children.Remove(btn.btn_door);
            }

        }

        private void place_curr_doors(int floor_selected)
        {
            // initialize empty list
            placed_doors = new List<button_struct>();

            // Load locked door image
            string locked_door_image = current_dir + folder + "\\closed_door.jpg";

            // Test margin stored correctly? NO
            //foreach (door_struct dd in jagged_doors[floor_selected])
            //{
              //  MessageBox.Show(floor_selected.ToString() + " "+  dd.door_margin[0].ToString());
            //}

            //
            //Button btn0 = new Button();
            //Button btn1 = new Button();

            //Button[] arr_button = new Button[] {btn0, btn1 };
            int i = 0;
            // create the specified number of doors for the current floor
            foreach (door_struct door_creating in flr_dr_class_lst[floor_selected].arr_doors)
            {

                string btn_name = "btn_Door_";
                string grd_name = "grd_Door_";
                string img_name = "img_Door_";

                // create personal button grid
                grd_name = grd_name + i.ToString();
                Grid door_grid = new Grid();
                door_grid.Name = grd_name + i.ToString();

                // set the margins for the grid
                Thickness grid_margin = door_grid.Margin;
                grid_margin.Left = 0;
                grid_margin.Top = 0;
                grid_margin.Right = 0;
                grid_margin.Bottom = 0;
                door_grid.Margin = grid_margin;
                //

                // name image, have to re-create object each loop to be able to add as a child
                Uri imageUri = new Uri(locked_door_image, UriKind.Absolute);   // do relative later
                BitmapImage imageBitmap = new BitmapImage(imageUri);
                Image my_doorImage = new Image();
                my_doorImage.Source = imageBitmap;
                my_doorImage.Height = 50;
                my_doorImage.Width = 50;
                Thickness img_margin = my_doorImage.Margin;
                img_margin.Left = 0;
                img_margin.Top = 0;
                img_margin.Right = 0;
                img_margin.Bottom = 0;
                my_doorImage.Margin = img_margin;

                img_name = img_name + i.ToString();
                my_doorImage.Name = img_name;
                //

                btn_name = btn_name + i.ToString();

                // test
                /*
                arr_button[i].Name = btn_name;
                arr_button[i].Height = 50;
                arr_button[i].Width = 50;
                arr_button[i].Background = Brushes.Red;
                arr_button[i].FontSize = 14;
                arr_button[i].Foreground = Brushes.White;
                arr_button[i].HorizontalAlignment = HorizontalAlignment.Left;
                arr_button[i].VerticalAlignment = VerticalAlignment.Top;

                Thickness door_margin = arr_button[i].Margin;
                door_margin.Left = door_creating.door_margin[0];
                door_margin.Top = door_creating.door_margin[1];
                door_margin.Right = door_creating.door_margin[2];
                door_margin.Bottom = door_creating.door_margin[3];
                arr_button[i].Margin = door_margin;

                arr_button[i].Click += generic_button_click;

                // Set grid as child of the button
                arr_button[i].Content = door_grid;

                button_struct btn_cr = new button_struct() { btn_door = arr_button[i], IP_door = door_creating.door_IP };
                 */
                // test end

                // set button attributes                
                Button btn_new = new Button();
                btn_new.Name = btn_name;

                //HorizontalAlignment="Left" Margin="215,202,0,0" VerticalAlignment="Top" Height ="50" Width="50" Click="btn_Door_1_Click" Background="Red" FontSize="14" Foreground="White"
                // Set default attributes
                btn_new.Height = 50;
                btn_new.Width = 50;
                btn_new.Background = Brushes.Red;
                btn_new.FontSize = 14;
                btn_new.Foreground = Brushes.White;
                btn_new.HorizontalAlignment = HorizontalAlignment.Left;
                btn_new.VerticalAlignment = VerticalAlignment.Top;

                // set the margins for the button
                Thickness door_margin = btn_new.Margin;
                door_margin.Left = door_creating.door_margin[0];
                door_margin.Top = door_creating.door_margin[1];
                door_margin.Right = door_creating.door_margin[2];
                door_margin.Bottom = door_creating.door_margin[3];
                btn_new.Margin = door_margin;

                // function for button click
                btn_new.Click += generic_button_click;

                // Set grid as child of the button
                btn_new.Content = door_grid;


                // Set Image as the child to the grid
                door_grid.Children.Add(my_doorImage);

                // Place doors within scrollviewer grid, only placing last read button
                grd_ScrollV.Children.Add(btn_new);

                // fill button_struct
                button_struct btn_cr = new button_struct() { btn_door = btn_new, IP_door = door_creating.door_IP };


                // fill array of buttons
                placed_doors.Add(btn_cr);

                i++;
            }

        }

        // On button click, send the udp message and change colour
        // Event handler for button click
        private void generic_button_click(object sender, RoutedEventArgs e)
        {
            string button_name = (sender as Button).Name.ToString();
            string button_IP = "";
            Button button_now = new Button();

            // search button_struct placed_doors() List for the door and get its IP
            foreach (button_struct btn in placed_doors)
            {
                //MessageBox.Show(btn.IP_door);
                string curr_name = btn.btn_door.Name;
                
                if (curr_name.Equals(button_name))
                {
                    
                    button_now = btn.btn_door;
                    button_IP = btn.IP_door;
                    break;
                }
            }

            //MessageBox.Show(button_IP);

            // udp send to open door
            send_udp_msg("222.33");

            // if ack receive that door is opened, change color of door to green for "opened" time
            btn_colour_change(button_now);

        }
        /*
        private void btn_Door_1_Click(object sender, RoutedEventArgs e)
        {
            // IPs and location of images are pulled from the DB, these names of the buttons are auto generated.
            //
            // udp send to open door
            send_udp_msg("222.33");

            // if ack receive that door is opened, change color of door to green for "opened" time
            btn_colour_change(btn_Door_1);


        }

        private void btn_Door_2_Click_1(object sender, RoutedEventArgs e)
        {
            // udp send to open door
            send_udp_msg("123.21");

            // if ack receive that door is opened, change color of door to green for "opened" time
            // The argument passed is auto generated.
            btn_colour_change(btn_Door_2);
        }
         */

        // Send UDP message to RPI to open door
        // Input: IP: string
        // output: success:int
        private int send_udp_msg(string doorIP)
        {
            return 0;
        }

        // Change colour of the button to green to indicate door has been opened, wait a time, and then change back to red
        private void btn_colour_change(Button btn_door)
        {
            btn_door.Background = Brushes.Green;   // Not filling due to new image not a child

            string open_door_path = current_dir + folder + "\\open_door.jpg";
            Uri imageUri = new Uri(open_door_path, UriKind.Absolute);   // do relative later
            BitmapImage imageBitmap = new BitmapImage(imageUri);
            Image myImage = new Image();
            myImage.Source = imageBitmap;

            /* Changing the image isnt working properly
            grd_Door_1.Children.Remove(img_door_1);

            grd_Door_1.Children.Add(myImage);

            // Change image within button
            Dispatcher.Invoke(() =>
            {
                btn_door.Content = myImage;
                // Set property or change UI compomponents. 
                //or_2.Source = myImage.Source;
            });
            
            */

            // task delay, time pulled from DB
            Task.Delay(time_door_open).ContinueWith(_ => 
                {
                    Dispatcher.Invoke(() =>
                        {
                            // Change color back to red to indicate door locked.
                            btn_door.Background = Brushes.Red;
                        }
                    );
                }
            );

          
        }

    }
}


