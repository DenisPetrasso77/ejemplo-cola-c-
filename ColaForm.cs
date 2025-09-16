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
        private Button btnVer;
        private Button btnBuscar;
        private Button btnCantidad;
        private Button btnVaciar;

        private Tuple<string, Color>[] opciones = new Tuple<string, Color>[] {
            Tuple.Create("Rojo", Color.Red),
            Tuple.Create("Verde", Color.Green),
            Tuple.Create("Azul", Color.Blue)
        };
        private int index = 0;

        // Tamaños
        private int panelWidth = 120;
        private int panelHeight = 60;
        private int espacio = 12;

        // Márgenes
        private int margenHorizontal = 30;
        private int margenVerticalSuperior = 30;
        private int margenVerticalInferior = 30;

        public ColaForm()
        {
            // Form principal
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Cola Animada con Estilo (Horizontal)";
            this.Size = new Size(880, 620);
            this.BackColor = Color.FromArgb(45, 45, 48);

            // Panel contenedor (horizontal)
            panelContenedor = new Panel()
            {
                Location = new Point(margenHorizontal, margenVerticalSuperior),
                Size = new Size(this.ClientSize.Width - margenHorizontal * 2,
                                150),
                BackColor = Color.White,
                AutoScroll = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelContenedor);

            // Botones debajo del panel
            int botonesTop = panelContenedor.Bottom + 30;

            btnEncolar = CrearBoton("➕ Enqueue", Color.RoyalBlue, botonesTop);
            btnEncolar.Click += BtnEncolar_Click;
            this.Controls.Add(btnEncolar);

            btnDesencolar = CrearBoton("➖ Dequeue", Color.Goldenrod, botonesTop + 60);
            btnDesencolar.Click += BtnDesencolar_Click;
            this.Controls.Add(btnDesencolar);

            btnVer = CrearBoton("👁️ Ver (Peek)", Color.MediumPurple, botonesTop + 120);
            btnVer.Click += BtnVer_Click;
            this.Controls.Add(btnVer);

            btnBuscar = CrearBoton("🔍 Buscar (Contains)", Color.IndianRed, botonesTop + 180);
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            btnCantidad = CrearBoton("📊 Cantidad", Color.DarkCyan, botonesTop + 240);
            btnCantidad.Click += BtnCantidad_Click;
            this.Controls.Add(btnCantidad);

            btnVaciar = CrearBoton("🗑️ Vaciar", Color.DarkRed, botonesTop + 300);
            btnVaciar.Click += BtnVaciar_Click;
            this.Controls.Add(btnVaciar);
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

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (cola.Count == 0)
            {
                MessageBox.Show("La cola está vacía.", "Buscar");
                return;
            }

            using (Form inputForm = new Form())
            {
                inputForm.Text = "Buscar en la Cola (Contains)";
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
                    bool existe = cola.Any(x => x.Item1.Equals(input, StringComparison.OrdinalIgnoreCase));

                    if (existe)
                    {
                        MessageBox.Show($"El elemento '{input}' existe en la cola ✅", "Buscar (Contains)");
                    }
                    else
                    {
                        MessageBox.Show($"El elemento '{input}' no existe en la cola ❌", "Buscar (Contains)");
                    }
                }
            }
        }

        private void BtnCantidad_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Cantidad de elementos en la cola: {cola.Count}", "Cantidad");
        }

        private void BtnVaciar_Click(object sender, EventArgs e)
        {
            cola.Clear();
            DibujarCola();
            MessageBox.Show("La cola fue vaciada correctamente.", "Vaciar");
        }

        // Dibuja toda la cola (horizontal, sin animaciones)
        private void DibujarCola()
        {
            panelContenedor.Controls.Clear();

            int x = 10;
            foreach (var item in cola)
            {
                var p = CrearPanelItem(item.Item1, item.Item2, x);
                panelContenedor.Controls.Add(p);
                x += panelWidth + espacio;
            }
        }

        // Dibuja con animación de entrada (horizontal)
        private void DibujarColaAnimadoEntrada(Tuple<string, Color> nuevo)
        {
            panelContenedor.Controls.Clear();
            int x = 10;

            int count = cola.Count;
            int i = 0;
            foreach (var item in cola)
            {
                if (i == count - 1)
                {
                    var p = CrearPanelItem(item.Item1, item.Item2, x);
                    p.Top = 10;
                    p.Left = -p.Width; // arranca fuera de pantalla
                    panelContenedor.Controls.Add(p);
                    AnimarEntrada(p, new Point(x, 10));
                }
                else
                {
                    var p = CrearPanelItem(item.Item1, item.Item2, x);
                    panelContenedor.Controls.Add(p);
                }
                x += panelWidth + espacio;
                i++;
            }
        }

        private Panel CrearPanelItem(string texto, Color color, int x)
        {
            Panel p = new Panel()
            {
                Size = new Size(panelWidth, panelHeight),
                BackColor = color,
                Location = new Point(x, 10),
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
