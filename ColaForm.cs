using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ColaAnimadaCool
{
    public class ColaForm : Form
    {
        private Queue<Tuple<string, Color>> cola = new Queue<Tuple<string, Color>>();

        private Panel panelContenedor;
        private Button btnEncolar;
        private Button btnDesencolar;
        private Button btnInfo;
        private Button btnVer;
        private Button btnBuscar;

        private Tuple<string, Color>[] opciones = new Tuple<string, Color>[] {
            Tuple.Create("Rojo", Color.Red),
            Tuple.Create("Verde", Color.Green),
            Tuple.Create("Azul", Color.Blue)
        };
        private int index = 0;

        // Tamaños
        private int panelHeight = 60;
        private int espacio = 12;

        // Márgenes
        private int margenHorizontal = 30;
        private int margenVerticalSuperior = 30;
        private int margenVerticalInferior = 30;

        public ColaForm()
        {
            // Form principal
            this.Text = "Cola Animada con Estilo";
            this.Size = new Size(520, 700);
            this.BackColor = Color.FromArgb(45, 45, 48);

            // Panel contenedor centrado con márgenes iguales
            panelContenedor = new Panel()
            {
                Location = new Point(margenHorizontal, margenVerticalSuperior),
                Size = new Size(this.ClientSize.Width - margenHorizontal * 2,
                                this.ClientSize.Height - 300 - margenVerticalSuperior - margenVerticalInferior),
                BackColor = Color.White,
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelContenedor);

            // Botones al fondo con margen inferior
            int botonesTop = panelContenedor.Bottom + 30;

            btnEncolar = CrearBoton("➕ Encolar", Color.RoyalBlue, botonesTop);
            btnEncolar.Click += BtnEncolar_Click;
            this.Controls.Add(btnEncolar);

            btnDesencolar = CrearBoton("➖ Desencolar", Color.Goldenrod, botonesTop + 60);
            btnDesencolar.Click += BtnDesencolar_Click;
            this.Controls.Add(btnDesencolar);

            //btnInfo = CrearBoton("ℹ️ Info de la Cola", Color.SeaGreen, botonesTop + 120);
            //this.Controls.Add(btnInfo);

            btnVer = CrearBoton("👁️ Ver (Peek)", Color.MediumPurple, botonesTop + 180);
            btnVer.Click += BtnVer_Click;
            this.Controls.Add(btnVer);

            btnBuscar = CrearBoton("🔍 Buscar", Color.IndianRed, botonesTop + 240);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);
        }

        private Button CrearBoton(string texto, Color color, int y)
        {
            Button b = new Button()
            {
                Text = texto,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(this.ClientSize.Width - margenHorizontal * 2, 50),
                Location = new Point(margenHorizontal, y),
                FlatStyle = FlatStyle.Flat
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void BtnEncolar_Click(object sender, EventArgs e)
        {
            var opcion = opciones[index];
            index = (index + 1) % opciones.Length;

            cola.Enqueue(opcion);
            DibujarColaAnimadoEntrada(opcion);
        }

        private void BtnDesencolar_Click(object sender, EventArgs e)
        {
            if (cola.Count == 0) return;
            var primerPanel = panelContenedor.Controls[0] as Panel;
            if (primerPanel != null)
            {
                AnimarSalida(primerPanel, () =>
                {
                    cola.Dequeue();
                    DibujarCola();
                });
            }
        }

        //private void BtnInfo_Click(object sender, EventArgs e)
        //{
        //    string inicio = cola.Count > 0 ? cola.Peek().Item1 : "Ninguno";
        //    string fin = cola.Count > 0 ? cola.ToArray()[cola.Count - 1].Item1 : "Ninguno";

        //    MessageBox.Show($"Tamaño: {cola.Count}\nInicio: {inicio}\nFin: {fin}", "Información de la Cola");
        //}

        // NUEVO: Ver (Peek)
        private void BtnVer_Click(object sender, EventArgs e)
        {
            if (cola.Count == 0)
            {
                MessageBox.Show("La cola está vacía.", "Ver (Peek)");
                return;
            }

            var primero = cola.Peek();
            MessageBox.Show($"El primer elemento es: {primero.Item1}", "Ver (Peek)");
        }

        // NUEVO: Buscar (con formulario propio)
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (cola.Count == 0)
            {
                MessageBox.Show("La cola está vacía.", "Buscar");
                return;
            }

            using (Form inputForm = new Form())
            {
                inputForm.Text = "Buscar en la Cola";
                inputForm.Size = new Size(300, 150);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                Label lbl = new Label()
                {
                    Text = "Ingrese el color a buscar:",
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                inputForm.Controls.Add(lbl);

                TextBox txt = new TextBox()
                {
                    Dock = DockStyle.Top,
                    Margin = new Padding(10),
                    TextAlign = HorizontalAlignment.Center
                };
                inputForm.Controls.Add(txt);

                Button btnOk = new Button()
                {
                    Text = "Buscar",
                    Dock = DockStyle.Bottom
                };
                inputForm.Controls.Add(btnOk);

                string input = null;
                btnOk.Click += (s, ev) =>
                {
                    input = txt.Text;
                    inputForm.DialogResult = DialogResult.OK;
                    inputForm.Close();
                };

                if (inputForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(input))
                {
                    var arr = cola.ToArray();
                    List<int> posiciones = new List<int>();

                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr[i].Item1.Equals(input, StringComparison.OrdinalIgnoreCase))
                        {
                            posiciones.Add(i + 1); // sumamos 1 porque la cola empieza en 1, no en 0
                        }
                    }

                    if (posiciones.Count > 0)
                    {
                        string lista = string.Join(", ", posiciones);
                        MessageBox.Show(
                            $"Elemento '{input}' encontrado en las posiciones: {lista}.",
                            "Buscar");
                    }
                    else
                    {
                        MessageBox.Show($"Elemento '{input}' no encontrado en la cola.", "Buscar");
                    }
                }

            }
        }

        // Dibuja toda la cola (sin animaciones)
        private void DibujarCola()
        {
            panelContenedor.Controls.Clear();

            int y = 10;
            foreach (var item in cola)
            {
                var p = CrearPanelItem(item.Item1, item.Item2, y);
                panelContenedor.Controls.Add(p);
                y += panelHeight + espacio;
            }
        }

        // Dibuja con animación de entrada
        private void DibujarColaAnimadoEntrada(Tuple<string, Color> nuevo)
        {
            panelContenedor.Controls.Clear();
            int y = 10;

            int count = cola.Count;
            int i = 0;
            foreach (var item in cola)
            {
                if (i == count - 1)
                {
                    // Este es el nuevo: animado
                    var p = CrearPanelItem(item.Item1, item.Item2, y);
                    // posición inicial fuera de pantalla
                    p.Left = -p.Width;
                    panelContenedor.Controls.Add(p);
                    AnimarEntrada(p, new Point(10, y));
                }
                else
                {
                    var p = CrearPanelItem(item.Item1, item.Item2, y);
                    panelContenedor.Controls.Add(p);
                }
                y += panelHeight + espacio;
                i++;
            }
        }

        private Panel CrearPanelItem(string texto, Color color, int y)
        {
            Panel p = new Panel()
            {
                Size = new Size(panelContenedor.Width - 20, panelHeight),
                BackColor = color,
                Location = new Point(10, y),
                BorderStyle = BorderStyle.None
            };

            Label lbl = new Label()
            {
                Text = texto,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            p.Controls.Add(lbl);
            return p;
        }

        private void AnimarEntrada(Panel p, Point destino)
        {
            Timer t = new Timer();
            t.Interval = 15;
            t.Tick += (s, e) =>
            {
                if (p.Left < destino.X)
                {
                    p.Left += 20;
                }
                else
                {
                    p.Left = destino.X;
                    t.Stop();
                    t.Dispose();
                }
            };
            t.Start();
        }

        private void AnimarSalida(Panel p, Action onFinish)
        {
            Timer t = new Timer();
            t.Interval = 15;
            t.Tick += (s, e) =>
            {
                if (p.Left < panelContenedor.Width)
                {
                    p.Left += 20;
                }
                else
                {
                    t.Stop();
                    t.Dispose();
                    panelContenedor.Controls.Remove(p);
                    p.Dispose();
                    onFinish?.Invoke();
                }
            };
            t.Start();
        }
    }
}
