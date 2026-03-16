using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RedSismicaWinForms
{
    public partial class PantRegistrarResultado : Form
    {
        private List<EventoSismico> eventos;
        private GestorRegistrarResultado gestor;
        private EventoSismico eventoSeleccionado;

        // Controles visuales
        private Panel panelHeader;
        private Label lblTitulo;
        private Label lblSubEventos, lblDetalles, lblSeries, lblAccion, lblMensaje;
        private ListBox listBoxEventos, listBoxSeries;
        private Button btnSeleccionar, btnRegistrar, btnVisualizarMapa, btnModificarDatos;
        private TextBox txtDetalles;
        private ComboBox comboAccion;


        public PantRegistrarResultado(GestorRegistrarResultado gestor)
        {
            this.gestor = gestor;
            InicializarControles();
            seleccionarRegistroResultadoRevisionManual();
            this.Resize += PantRegistrarResultado_Resize;
        }

        private void InicializarControles()
        {
            // Ventana
            this.Text = "Revisión manual de eventos sísmicos";
            this.MinimumSize = new Size(1100, 680);
            this.Size = new Size(1200, 750);
            this.BackColor = Color.FromArgb(248, 250, 255);

            // Header superior
            panelHeader = new Panel
            {
                Left = 0,
                Top = 0,
                Width = this.Width,
                Height = 65,
                BackColor = Color.FromArgb(44, 62, 80),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelHeader);

            lblTitulo = new Label
            {
                Text = "🟦 Registro de revisión manual de eventos sísmicos",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Left = 36,
                Top = 13,
                AutoSize = true
            };
            panelHeader.Controls.Add(lblTitulo);

            // Subtítulo lista eventos
            lblSubEventos = new Label
            {
                Text = "Eventos sísmicos auto detectados",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                BackColor = Color.Transparent,
                Left = 42,
                Top = 82,
                Width = 510
            };
            // ListBox eventos (más ancho y alto)
            listBoxEventos = new ListBox
            {
                Left = 40,
                Top = 115,
                Width = 700,
                Height = 240,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(52, 73, 94),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            listBoxEventos.HorizontalScrollbar = true;

            // Botón seleccionar evento
            btnSeleccionar = new Button
            {
                Text = "Seleccionar evento",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Left = 40,
                Top = 370,
                Width = 510,
                Height = 40,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            btnSeleccionar.FlatAppearance.BorderSize = 0;
            btnSeleccionar.Cursor = Cursors.Hand;
            btnSeleccionar.Click += btnSeleccionar_Click;

            // Botón visualizar mapa (debajo de seleccionar)
            btnVisualizarMapa = new Button
            {
                Text = "Mapa",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Left = 40,
                Top = 420,
                Width = 510,
                Height = 40,
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            btnVisualizarMapa.FlatAppearance.BorderSize = 0;
            btnVisualizarMapa.Cursor = Cursors.Hand;
            btnVisualizarMapa.Click += btnSeleccionar_Mapa;

            btnModificarDatos = new Button
            {
                Text = "Modificar datos del evento",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Left = 40,
                Top = 470, // Debajo de Mapa
                Width = 510,
                Height = 40,
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            btnModificarDatos.FlatAppearance.BorderSize = 0;
            btnModificarDatos.Cursor = Cursors.Hand;
            btnModificarDatos.Click += btnModificarDatos_Click;
            this.Controls.Add(btnModificarDatos);

            // Subtítulo detalles
            lblDetalles = new Label
            {
                Text = "Detalle del evento",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                BackColor = Color.Transparent,
                Left = 40,
                Top = 475,
                Width = 510
            };

            // TextBox detalles (más ancho)
            txtDetalles = new TextBox
            {
                Left = 40,
                Top = 505,
                Width = 510,
                Height = 100,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(240, 243, 249),
                ForeColor = Color.FromArgb(33, 37, 41),
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = ScrollBars.Vertical
            };

            // Subtítulo series (más a la derecha)
            lblSeries = new Label
            {
                Text = "Series temporales asociadas",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                BackColor = Color.Transparent,
                Left = 580,
                Top = 82,
                Width = 240
            };

            // ListBox series (más angosto)
            listBoxSeries = new ListBox
            {
                Left = 580,
                Top = 115,
                Width = 250,
                Height = 250,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(52, 73, 94),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            // Subtítulo acción
            lblAccion = new Label
            {
                Text = "Acción sobre el evento:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                BackColor = Color.Transparent,
                Left = 40,
                Top = 630,
                Width = 250
            };

            // Combo acción
            comboAccion = new ComboBox
            {
                Left = 40,
                Top = 660,
                Width = 260,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboAccion.Items.AddRange(new string[] { "Confirmar", "Rechazar", "Solicitar revisión a experto" });
            comboAccion.SelectedIndex = 0;

            // Botón registrar
            btnRegistrar = new Button
            {
                Text = "Registrar resultado",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Left = 320,
                Top = 570,
                Width = 200,
                Height = 40,
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            btnRegistrar.FlatAppearance.BorderSize = 0;
            btnRegistrar.Cursor = Cursors.Hand;
            btnRegistrar.Click += btnRegistrar_Click;

            // Label mensaje
            lblMensaje = new Label
            {
                Left = 580,
                Top = 630,
                Width = 420,
                Height = 60,
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.FromArgb(39, 174, 96),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.Transparent
            };

            // Agrego todos los controles
            this.Controls.Add(lblSubEventos);
            this.Controls.Add(listBoxEventos);
            this.Controls.Add(btnSeleccionar);
            this.Controls.Add(btnVisualizarMapa);
            this.Controls.Add(lblDetalles);
            this.Controls.Add(txtDetalles);
            this.Controls.Add(lblSeries);
            this.Controls.Add(listBoxSeries);
            this.Controls.Add(lblAccion);
            this.Controls.Add(comboAccion);
            this.Controls.Add(btnRegistrar);
            this.Controls.Add(lblMensaje);
        }

        // Lógica responsive: ajusta controles al redimensionar
        private void PantRegistrarResultado_Resize(object sender, EventArgs e)
        {
            int margenIzq = 40, espacioCol = 30;
            int anchoVentana = this.ClientSize.Width;
            int altoVentana = this.ClientSize.Height;
            int alturaHeader = panelHeader.Height;

            // Columna izquierda
            listBoxEventos.Width = 800;
            btnSeleccionar.Width = 510;
            btnVisualizarMapa.Width = 510;
            txtDetalles.Width = 510;
            lblDetalles.Width = 400;

            btnSeleccionar.Top = listBoxEventos.Bottom + 15;
            btnVisualizarMapa.Top = btnSeleccionar.Bottom + 10;
            lblDetalles.Top = btnVisualizarMapa.Bottom + 15;
            txtDetalles.Top = lblDetalles.Bottom + 5;

            lblAccion.Top = altoVentana - 90;
            comboAccion.Top = lblAccion.Bottom + 4;
            btnRegistrar.Top = comboAccion.Top;

            // Columna derecha (series)
            int colDerLeft = margenIzq + listBoxEventos.Width + espacioCol;
            listBoxSeries.Left = lblSeries.Left = lblMensaje.Left = colDerLeft;
            listBoxSeries.Width = anchoVentana - colDerLeft - margenIzq;
            listBoxSeries.Height = altoVentana - 160;
            lblSeries.Top = 82;

            // Feedback
            lblMensaje.Top = altoVentana - 90;
            lblMensaje.Width = listBoxSeries.Width;
        }
        private void btnModificarDatos_Click(object sender, EventArgs e)
        {
            int indice = pedirSeleccionEvento();
            if (indice < 0)
            {
                MessageBox.Show("Seleccioná un evento primero.", "Advertencia");
                return;
            }

            // Obtengo los datos actuales del evento (usá métodos del gestor)
            var evento = eventos[indice];
            string alcanceActual = evento.getAlcance()?.ToString() ?? "";
            string magnitudActual = evento.getValorMagnitud().ToString();
            string origenActual = evento.getOrigenDeGeneracion()?.ToString() ?? "";

            // Pido los nuevos datos (podrías usar un form custom, acá es simple)
            string nuevoAlcance = Prompt.ShowDialog("Nuevo alcance:", "Modificar Alcance", alcanceActual);
            if (nuevoAlcance == null) return;
            string nuevaMagnitudStr = Prompt.ShowDialog("Nueva magnitud:", "Modificar Magnitud", magnitudActual);
            if (nuevaMagnitudStr == null) return;
            string nuevoOrigen = Prompt.ShowDialog("Nuevo origen:", "Modificar Origen", origenActual);
            if (nuevoOrigen == null) return;

            // Validaciones simples
            if (!double.TryParse(nuevaMagnitudStr, out double nuevaMagnitud))
            {
                MessageBox.Show("La magnitud debe ser numérica.", "Error");
                return;
            }

            // Llamo al gestor para modificar
            gestor.modificarDatosEvento(indice, nuevoAlcance, nuevaMagnitud, nuevoOrigen);

            MessageBox.Show("Datos modificados correctamente.", "Éxito");
            mostrarDatosSismicos(indice); // Refrescá los datos en pantalla
        }

        // Clase utilitaria para InputBox (pegar en cualquier lado de tu proyecto)
        public static class Prompt
        {
            public static string ShowDialog(string text, string caption, string defaultValue = "")
            {
                Form prompt = new Form()
                {
                    Width = 400,
                    Height = 170,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };
                Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 340 };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340, Text = defaultValue };
                Button confirmation = new Button() { Text = "Aceptar", Left = 200, Width = 80, Top = 90, DialogResult = DialogResult.OK };
                Button cancel = new Button() { Text = "Cancelar", Left = 280, Width = 80, Top = 90, DialogResult = DialogResult.Cancel };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                cancel.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = confirmation;
                prompt.CancelButton = cancel;
                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : null;
            }
        }

        // Métodos funcionales requeridos
        public void seleccionarRegistroResultadoRevisionManual()
        {
            habilitarVentana();
            eventos = gestor.tomarRegistroResultadoRevisionManual();
            mostrarEventosOrdenados(eventos);
            txtDetalles.Text = "";
            listBoxSeries.Items.Clear();
            lblMensaje.Text = "";
        }
        public void habilitarVentana() => this.Enabled = true;

        public void mostrarEventosOrdenados(List<EventoSismico> eventos)
        {
            listBoxEventos.Items.Clear();
            foreach (var ev in eventos)
                listBoxEventos.Items.Add(gestor.buscarDatosEventoSismico(ev));
        }

        public int pedirSeleccionEvento() => listBoxEventos.SelectedIndex;

        public void tomarSeleccionEvento()
        {
            int indice = pedirSeleccionEvento();
            if (indice < 0)
            {
                lblMensaje.Text = "Seleccioná un evento para ver detalles.";
                lblMensaje.ForeColor = Color.FromArgb(192, 57, 43);
                return;
            }
            gestor.tomarSeleccionEvento(eventos[indice]); // si tu gestor lo necesita, sino por índice
            mostrarDatosSismicos(indice);
            lblMensaje.Text = "";
        }

        public void mostrarDatosSismicos(int indice)
        {
            txtDetalles.Text = gestor.buscarDatosEventoSismico(indice);
            listBoxSeries.Items.Clear();
            foreach (var serie in gestor.buscarSeriesTemporales(indice))
                listBoxSeries.Items.Add(serie);
        }

        public void mostrarOpcionVisualizarMapa()
        {
            var resultado = MessageBox.Show(
                "¿Deseás visualizar el mapa del evento seleccionado?",
                "Visualizar Mapa",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.OK)
            {
                MessageBox.Show("Mapa abierto (simulado).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public string pedirAccionConEvento() => comboAccion.SelectedItem?.ToString();

        public void tomarAccionConEvento()
        {
            string accion = pedirAccionConEvento();
            gestor.tomarAccionConEvento(accion);
            lblMensaje.Text = "✔️ Evento actualizado correctamente.";
            lblMensaje.ForeColor = Color.FromArgb(39, 174, 96);
            CargarEventos();
            comboAccion.SelectedIndex = 0;
            eventoSeleccionado = null;
        }

        // Handlers
        private void btnSeleccionar_Click(object sender, EventArgs e) => tomarSeleccionEvento();
        private void btnSeleccionar_Mapa(object sender, EventArgs e) => mostrarOpcionVisualizarMapa();

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (eventoSeleccionado == null)
            {
                lblMensaje.Text = "Seleccioná un evento y mostrá detalles primero.";
                lblMensaje.ForeColor = Color.FromArgb(192, 57, 43);
                return;
            }
            if (string.IsNullOrEmpty(pedirAccionConEvento()))
            {
                lblMensaje.Text = "Seleccioná una acción.";
                lblMensaje.ForeColor = Color.FromArgb(192, 57, 43);
                return;
            }
            tomarAccionConEvento(); 
        }


        private void CargarEventos()
        {
            eventos = gestor.obtenerEventosSismicosAutoDetectados();
            mostrarEventosOrdenados(eventos);
            txtDetalles.Text = "";
            listBoxSeries.Items.Clear();
            lblMensaje.Text = "";
        }
    }
}
