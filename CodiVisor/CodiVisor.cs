using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.IO;
using System.Data.SqlClient; 

namespace CodiVisor
{
    public partial class CodiVisor : Form
    {
        public CodiVisor()
        {
            InitializeComponent();
        }

        QrCodeEncodingOptions opciones;
        string connectionString = "Data Source= DESKTOP-J5P6950;Initial Catalog=CodiVisor;Integrated Security=True;";




        private void CodiVisor_Load(object sender, EventArgs e)
        {
            opciones = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 600,
                Height = 600,

            };
        }

        private void btnGenerarCodigo_Click(object sender, EventArgs e)
        {
            var textoQr = new ZXing.BarcodeWriter();
            textoQr.Options= opciones;
            textoQr.Format = ZXing.BarcodeFormat.QR_CODE;
            var resultado = new Bitmap(textoQr.Write(txtTexto.Text.Trim()));
            picQRCode.Image = resultado;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            SaveFileDialog guardar = new SaveFileDialog();

            guardar.CreatePrompt = true;
            guardar.OverwritePrompt = true;
            guardar.FileName = txtTexto.Text;
            guardar.Filter = "Achivos PNG (.png) | *.png";
            if(guardar.ShowDialog() == DialogResult.OK)
            {
                picQRCode.Image.Save(guardar.FileName);
                guardar.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Codigo_QR (Nombre_Escuela, QR_Text, QR_Image) VALUES (@NombreEscuela, @QRText, @QRImage)";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Asignar valores a los parámetros
                    command.Parameters.AddWithValue("@NombreEscuela", txtEscuelas.Text.Trim());
                    command.Parameters.AddWithValue("@QRText", txtTexto.Text.Trim());


                    // Convertir la imagen a un arreglo de bytes para almacenarla en la base de datos
                    using (MemoryStream ms = new MemoryStream())
                    {
                        picQRCode.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] qrImageData = ms.ToArray();
                        command.Parameters.AddWithValue("@QRImage", qrImageData);

                    }
                    connection.Open();
                    command.ExecuteNonQuery();

                }
                MessageBox.Show("Datos guardados correctamente.", "Éxito");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error: " + ex.Message, "Error");
            }















        }

        

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtEscuelas_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
