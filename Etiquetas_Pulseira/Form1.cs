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
namespace Etiquetas_Nova_Versao
{
    public partial class Form1 : Form
    {
       

        int status;
        Paciente paciente;
       // Internacao internacao;
      //  List<Internacao> internacoes;
        string error;
        //string Andar ="";
        ///string Quarto ="";
        //string Leito ="";
       // string Clinica="";
        int starty = 5;//distancia das linhas



        int startXEsquerda = 275;
     
        public class Paciente
        {
            public string cd_prontuario { get; set; }
            public string nm_nome { get; set; }
            public string dt_data_nascimento { get; set; }
            public string nm_mae { get; set; }
       
           
        }
      /*  public class Internacao
        {
            public string cd_prontuario { get; set; }
            public string nr_leito { get; set; }
            public string dt_alta_medica { get; set; }
            public string nm_especialidade { get; set; }

        }*/
        public Form1()
        {
            InitializeComponent();

         
        }



        private void btImprimir_Click(object sender, EventArgs e)
        {
            try
            {

                string rh = txbRh.Text;
                //detiq = dados.getDados(be);
                string url = "http://10.48.21.64:5003/hspmsgh-api/pacientes/paciente/" + rh;
                WebRequest request = WebRequest.Create(url);
                try
                {
                    using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
                    {
                        using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                        {
                            JsonSerializer json = new JsonSerializer();
                            var objText = reader.ReadToEnd();
                            paciente = JsonConvert.DeserializeObject<Paciente>(objText);

                        }
                    }

                /*    string url2 = "http://10.48.21.64:5003/hspmsgh-api/internacoes/" + rh;
                    WebRequest request2 = WebRequest.Create(url2);
                    using (var twitpicResponse2 = (HttpWebResponse)request2.GetResponse())
                    {
                        using (var reader2 = new StreamReader(twitpicResponse2.GetResponseStream()))
                        {
                            JsonSerializer json2 = new JsonSerializer();
                            var objText2 = reader2.ReadToEnd();
                            internacoes = JsonConvert.DeserializeObject<List<Internacao>>(objText2);

                        }
                    }

                    if (internacoes.Count != 0)
                    {
                        if (internacoes[0].dt_alta_medica == null)

                        Andar = internacoes[0].nr_leito == null ? "" : internacoes[0].nr_leito.Substring(0, 2);
                        Quarto = internacoes[0].nr_leito == null ? "" : internacoes[0].nr_leito.Substring(2, 2);
                        Leito = internacoes[0].nr_leito == null ? "" : internacoes[0].nr_leito.Substring(5, 2);
                        Clinica = internacoes[0].nm_especialidade == null ? "" : internacoes[0].nm_especialidade;


                    }

                    */

                   // PrintDialog printDialog1 = new PrintDialog();
//printDialog1.Document = printDocument1;  
                   // int widthInHundredthsOfInch = (int)(23 / 25.4 * 100);
                  // int heightInHundredthsOfInch = (int)(270 / 25.4 * 100);
                   printDocument1.PrinterSettings.PrinterName = "PrintPulseira";
                    // Set the custom paper size
                 //   printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom", widthInHundredthsOfInch, heightInHundredthsOfInch);

                  


                


                    // Set margins to zero if not already set
                   // printDocument1.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);

                    // Optional: Set printable area if necessary
                   // printDocument1.OriginAtMargins = false; // Set to true if you want the origin to be at margins     

                    //DialogResult result = printDialog1.ShowDialog();    
                      
                                printDocument1.Print();
                         
                     
                  


                    txbRh.Text = "";
                  //  Andar = "";
                  //  Quarto = "";
                   // Leito = "";
                   // Clinica = "";



            }
                catch (Exception ex)
                {
                    MessageBox.Show("Número de RH inexistente! " + ex.Message);
                    status = 1;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Número de RH inexistente! " + ex.Message);
                status = 1;

            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btImprimir.Enabled = true;
            if (status == 1)
                lblError.Text = error;
            else
                lblError.ResetText();



        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }



        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
           
          
            

           



            //DateTime dt = DateTime.Now;
            //string data = dt.ToString("g");  

           // e.PageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 30, 240);//900 é a largura da página
            //printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 500, 1000);
           // printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 500, 1000);
            Graphics g = e.Graphics;


            /*2. Print barcode...
            using (Neodynamic.SDK.Barcode.BarcodeProfessional barcode = new Neodynamic.SDK.Barcode.BarcodeProfessional())
            {
                //set barcode symbology... eg Code 128
                barcode.Symbology = Neodynamic.SDK.Barcode.Symbology.Code128;
                //set barcode bars dimensions...
                barcode.BarcodeUnit = Neodynamic.SDK.Barcode.BarcodeUnit.Inch;
                barcode.BarWidth = 0.010;
                barcode.BarHeight = 0.32;
                //set value to encode... this time a random value...
                barcode.Code = paciente.cd_prontuario;
                //justify text...
                barcode.CodeAlignment = Neodynamic.SDK.Barcode.Alignment.BelowJustify;
                //set font for barcode human readable text...
                using (Font fnt = new Font("Courier New", 10f))
                {
                    barcode.Font = fnt;
                }
                //print barcode below logo...
                //IMPORTANT: barcode location unit depends on BarcodeUnit setting which in this case is INCH
               // barcode.DrawOnCanvas(e.Graphics, new PointF(0.1f, 1.5f));
                barcode.DrawOnCanvas(e.Graphics);
            }*/

            Image newImage = Code128Rendering.MakeBarcodeImage(paciente.cd_prontuario, 1, true);

           // newImage.RotateFlip(RotateFlipType.Rotate90FlipY);

            // Create coordinates for upper-left corner.

           int x =120; // antes -8
            int y = 20;

            int width = 150; // com 180 funciona corretamente na maquina 
            int height = 30; // 40 antes

            // Draw image to screen.
           
            g.DrawImage(newImage, x, y, width, height);







          g.DrawString("PRONTUARIO: " + paciente.cd_prontuario, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty);
          g.DrawString("PACIENTE: " + paciente.nm_nome, new Font("Arial", 8, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 15);
          g.DrawString("NASC.: " + paciente.dt_data_nascimento, new Font("Arial", 8 , FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 30);
            g.DrawString("MAE: " + paciente.nm_mae, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 45);

        }

        private void txbRh_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Enter)
            {

                btImprimir_Click(sender, e);

            }
        }
    }
}