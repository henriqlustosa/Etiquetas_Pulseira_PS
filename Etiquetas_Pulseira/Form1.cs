using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Drawing.Printing;
using GenCode128;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;

namespace Etiquetas_Nova_Versao
{
    public partial class Form1 : Form
    {
        private int status;
        private Paciente paciente;
        private string error = "";

        // Layout positioning
        private readonly int starty = 20;  // Vertical offset for text
        private readonly int startXEsquerda = 275;  // Horizontal offset for text

        // Example: wristband/pulseira size in mm
        private const int LabelWidthMm = 23;
        private const int LabelHeightMm = 270;

        public class Paciente
        {
            public string cd_prontuario { get; set; }
            public string nm_nome { get; set; }
            public string dt_data_nascimento { get; set; }
            public string nm_mae { get; set; }
            public string nm_nome_social { get; set; }
            public string nm_pai { get; set; }

        }

        public Form1()
        {
            InitializeComponent();

            // You may want to configure your PrintDocument here:
            printDocument1.PrintPage += printDocument1_PrintPage;
        }

        private void btImprimir_Click(object sender, EventArgs e)
        {
            string rh = txbRh.Text;
            if (string.IsNullOrEmpty(rh) || rh.Trim().Length == 0)
            {
                MessageBox.Show("Por favor, insira um número de RH válido.");
                return;
            }

            // Attempt to retrieve data from API
            if (!FetchPacienteData(rh))
            {
                // If there was an error, we don’t proceed.
                return;
            }

            // Configure the printing
            try
            {
                // Convert millimeters to hundredths of an inch
                int widthInHundredthsOfInch = (int)(LabelWidthMm * 3.937007874015748);
                int heightInHundredthsOfInch = (int)(LabelHeightMm * 3.937007874015748);

                // Create a custom PaperSize
                // Note: Not all printers/drivers accept arbitrary custom sizes.
                PaperSize customSize = new PaperSize("CustomWristband",
                                                     widthInHundredthsOfInch,
                                                     heightInHundredthsOfInch);
                // Assign to the PrintDocument
                printDocument1.DefaultPageSettings.PaperSize = customSize;

                // You can remove margins (if needed) and set orientation
                printDocument1.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                // If your wristband is wide but short, you might use Landscape. Otherwise, set to false.
                printDocument1.DefaultPageSettings.Landscape = true;

                // Set the printer name here. Make sure it matches an installed printer.
                printDocument1.PrinterSettings.PrinterName = "PrinterPulPSO";
                //printDocument1.PrinterSettings.PrinterName = "HP TI";
                // Optionally, show a PrintDialog if you want the user to confirm
                // printDialog1.Document = printDocument1;
                // if (printDialog1.ShowDialog() != DialogResult.OK) return;

                // Print
                printDocument1.Print();

                // Clear input box after successful print
                txbRh.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao imprimir. Verifique se a impressora está correta.\n" + ex.Message);
                status = 1;
            }
        }

        /// <summary>
        /// Fetch Paciente data from your API endpoint.
        /// </summary>
        private bool FetchPacienteData(string rh)
        {
            string url = "http://10.48.21.64:5003/hspmsgh-api/pacientes/paciente/" + rh;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // If the status code is not OK, handle it
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        MessageBox.Show("Número de RH inexistente ou erro na API: " + response.StatusCode);
                        return false;
                    }

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonText = reader.ReadToEnd();
                        paciente = JsonConvert.DeserializeObject<Paciente>(jsonText);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao consultar o paciente: " + ex.Message);
                status = 1;
                return false;
            }
        }
        // A simple struct to hold the split results.
        private struct StringSplit
        {
            public string FirstPart;
            public string SecondPart;
        }

        // Helper method to split a string based on a maximum length
        private StringSplit SplitStringByLength(string value, int maxLength)
        {
            var result = new StringSplit();

            if (string.IsNullOrEmpty(value))
            {
                result.FirstPart = string.Empty;
                result.SecondPart = string.Empty;
            }
            else if (value.Length <= maxLength)
            {
                result.FirstPart = value;
                result.SecondPart = string.Empty;
            }
            else
            {
                result.FirstPart = value.Substring(0, maxLength);
                result.SecondPart = value.Substring(maxLength);
            }

            return result;
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {


            // Define some constants for maximum string lengths
            const int PATIENT_NAME_MAX_LENGTH = 16;
            const int MOTHER_NAME_MAX_LENGTH = 24;


            // Retrieve data from the paciente object
            string prontuario = paciente.cd_prontuario;
            string nomeSocial = paciente.nm_nome_social ?? string.Empty;
            string nomePaciente = paciente.nm_nome ?? string.Empty;
            string nomeMae = paciente.nm_mae ?? string.Empty;
            string nomePai = paciente.nm_pai ?? string.Empty;
            string dataNascimento = paciente.dt_data_nascimento; // Already formatted?

            // If there's a nome social, concatenate it
            if (!string.IsNullOrEmpty(nomeSocial))
            {
                nomePaciente = nomeSocial + " (" + nomePaciente + ")";
            }

            // If the mother's name is "DESCONHECIDA", use the father's name
            if (nomeMae.Equals("DESCONHECIDA", StringComparison.OrdinalIgnoreCase))
            {
                nomeMae = nomePai;
            }

            // Split patient name if longer than the max length
            StringSplit pacienteSplit = SplitStringByLength(nomePaciente, PATIENT_NAME_MAX_LENGTH);
            string pacientePart1 = pacienteSplit.FirstPart;
            string pacientePart2 = pacienteSplit.SecondPart;

            // Split mother's name if longer than the max length
            StringSplit maeSplit = SplitStringByLength(nomeMae, MOTHER_NAME_MAX_LENGTH);
            string maePart1 = maeSplit.FirstPart;
            string maePart2 = maeSplit.SecondPart;
            if (paciente == null)
            {
                // In case there's no valid data
                e.Cancel = true;
                return;
            }

            Graphics g = e.Graphics;

            // Generate Code128 barcode image
            // Make sure you dispose the barcode image after drawing
            using (Image barcodeImage = Code128Rendering.MakeBarcodeImage(paciente.cd_prontuario, 1, true))
            {
                int x = 120;
                int y = 30;
                int width = 150;
                int height = 30;

                // Draw the barcode
                g.DrawImage(barcodeImage, x, y, width, height);
            }




            // Draw text
            // Tip: You might want to measure your strings or further refine positions
            Font regularFont = new Font("Arial", 8, FontStyle.Regular);
            Font boldFont = new Font("Arial", 8, FontStyle.Bold);

            g.DrawString("PRONTUARIO: " + prontuario, regularFont, Brushes.Black, startXEsquerda, starty);
            g.DrawString("PACIENTE: " + nomePaciente, boldFont, Brushes.Black, startXEsquerda, starty + 15);
            g.DrawString("NASC.: " + dataNascimento,
                         regularFont, Brushes.Black, startXEsquerda, starty + 30);
            g.DrawString("MAE: " + nomeMae,
                         regularFont, Brushes.Black, startXEsquerda, starty + 45);
        }

        private void txbRh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btImprimir_Click(sender, e);
            }
        }

        // If you still need admin rights to set registry entries, consider:
        // 1) Checking if user is admin
        // 2) Restarting app as admin
        // 3) Handling "Access Denied" gracefully
        // but for many label printers, simply defining a custom PaperSize or
        // a custom Form in Windows is enough.
    }
}
