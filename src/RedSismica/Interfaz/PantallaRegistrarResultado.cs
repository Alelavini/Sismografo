using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace RedSismica
{
    // Pantalla del caso de uso "Registrar resultado de revisión manual"
    public class PantallaRegistrarResultado : Form
    {
        private readonly GestorRegistrarResultado gestor;
        private List<EventoSismico> eventos = new();
        private bool cargandoGrilla;

        private DataGridView gridEventos = null!;
        private DataGridView gridSeries = null!;
        private DataGridView gridHistorial = null!;
        private Label lblSinSeleccion = null!;
        private TableLayoutPanel tablaDetalle = null!;
        private readonly Dictionary<string, Label> valoresDetalle = new();
        private Button btnModificarDatos = null!, btnVerMapa = null!, btnRegistrar = null!;
        private ComboBox comboAccion = null!;
        private ToolStripStatusLabel lblEstado = null!, lblPendientes = null!;

        private static readonly Color ColorTexto = Color.FromArgb(33, 37, 41);
        private static readonly Color ColorSecundario = Color.FromArgb(108, 117, 125);
        private static readonly Color ColorFondo = Color.FromArgb(243, 244, 246);
        private static readonly Color ColorError = Color.FromArgb(176, 42, 55);
        private static readonly Color ColorExito = Color.FromArgb(25, 111, 61);

        private static readonly string[] CamposDetalle =
            { "Fecha y hora", "Magnitud", "Clasificación", "Epicentro", "Hipocentro", "Alcance", "Origen", "Estado" };

        public PantallaRegistrarResultado(GestorRegistrarResultado gestor)
        {
            this.gestor = gestor;
            InicializarControles();
            FormClosing += (_, _) => gestor.liberarEventoSeleccionado();
            Shown += (_, _) => seleccionarRegistroResultadoRevisionManual();
        }

        // ---------- Construcción de la interfaz ----------

        private void InicializarControles()
        {
            Text = "Red Sismológica - Revisión manual de eventos";
            Font = new Font("Segoe UI", 9.75f);
            ForeColor = ColorTexto;
            BackColor = ColorFondo;
            AutoScaleMode = AutoScaleMode.Font;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1240, 780);
            MinimumSize = new Size(1040, 660);

            var raiz = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Padding = new Padding(12, 8, 12, 8) };
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
            raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            raiz.Controls.Add(CrearColumnaIzquierda(), 0, 0);
            raiz.Controls.Add(CrearColumnaDerecha(), 1, 0);

            Controls.Add(raiz);
            Controls.Add(CrearEncabezado());
            Controls.Add(CrearBarraEstado());
        }

        private Control CrearEncabezado()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White, Padding = new Padding(20, 10, 20, 8) };
            panel.Paint += (_, e) => e.Graphics.DrawLine(new Pen(Color.FromArgb(222, 226, 230)), 0, panel.Height - 1, panel.Width, panel.Height - 1);

            var titulo = new Label
            {
                Text = "Registrar resultado de revisión manual",
                Font = new Font("Segoe UI Semibold", 14f),
                AutoSize = true,
                Location = new Point(18, 9)
            };
            var subtitulo = new Label
            {
                Text = "Eventos sísmicos detectados automáticamente por la red, pendientes de revisión",
                ForeColor = ColorSecundario,
                AutoSize = true,
                Location = new Point(20, 37)
            };
            var usuario = new Label
            {
                Text = $"Analista: {gestor.obtenerUsuarioLogueado().getAnalistaEnSismos()}",
                ForeColor = ColorSecundario,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            panel.Controls.AddRange(new Control[] { titulo, subtitulo, usuario });
            panel.Layout += (_, _) => usuario.Location = new Point(panel.ClientSize.Width - usuario.Width - 20, 24);
            return panel;
        }

        private Control CrearBarraEstado()
        {
            var barra = new StatusStrip { SizingGrip = true, BackColor = Color.White };
            lblEstado = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            lblPendientes = new ToolStripStatusLabel { ForeColor = ColorSecundario };
            barra.Items.Add(lblEstado);
            barra.Items.Add(lblPendientes);
            return barra;
        }

        private Control CrearColumnaIzquierda()
        {
            var columna = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, Margin = new Padding(0, 0, 6, 0) };
            columna.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            columna.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

            gridEventos = CrearGrilla();
            gridEventos.Columns.Add("fecha", "Fecha y hora");
            gridEventos.Columns.Add("magnitud", "Magnitud");
            gridEventos.Columns.Add("clasificacion", "Clasificación");
            gridEventos.Columns.Add("epicentro", "Epicentro (lat, long)");
            gridEventos.Columns.Add("estado", "Estado");
            gridEventos.Columns["magnitud"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            gridEventos.Columns["magnitud"]!.FillWeight = 60;
            gridEventos.SelectionChanged += gridEventos_SelectionChanged;

            gridSeries = CrearGrilla();
            gridSeries.Columns.Add("estacion", "Estación");
            gridSeries.Columns.Add("sismografo", "Sismógrafo");
            gridSeries.Columns.Add("muestras", "Muestras");
            gridSeries.Columns.Add("velocidad", "Vel. máx. (km/s)");
            gridSeries.Columns.Add("alarma", "Alarma");
            gridSeries.Columns["estacion"]!.FillWeight = 170;
            gridSeries.Columns["muestras"]!.FillWeight = 50;
            gridSeries.Columns["alarma"]!.FillWeight = 50;
            gridSeries.Columns["velocidad"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            columna.Controls.Add(Agrupar("Eventos auto detectados", gridEventos), 0, 0);
            columna.Controls.Add(Agrupar("Series temporales del evento", gridSeries), 0, 1);
            return columna;
        }

        private Control CrearColumnaDerecha()
        {
            var columna = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Margin = new Padding(6, 0, 0, 0) };
            columna.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            columna.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            columna.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));

            // Detalle del evento
            tablaDetalle = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(4) };
            tablaDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            tablaDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (var campo in CamposDetalle)
            {
                tablaDetalle.Controls.Add(new Label { Text = campo, ForeColor = ColorSecundario, AutoSize = true, Margin = new Padding(3, 4, 3, 4) });
                var valor = new Label { AutoSize = true, Margin = new Padding(3, 4, 3, 4) };
                valoresDetalle[campo] = valor;
                tablaDetalle.Controls.Add(valor);
            }

            lblSinSeleccion = new Label
            {
                Text = "Seleccioná un evento de la lista para revisarlo.",
                ForeColor = ColorSecundario,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnModificarDatos = new Button { Text = "Modificar datos...", AutoSize = true, Padding = new Padding(6, 2, 6, 2) };
            btnModificarDatos.Click += btnModificarDatos_Click;
            btnVerMapa = new Button { Text = "Ver en mapa", AutoSize = true, Padding = new Padding(6, 2, 6, 2) };
            btnVerMapa.Click += (_, _) => mostrarOpcionVisualizarMapa();
            var botonesDetalle = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, Padding = new Padding(2, 4, 0, 2) };
            botonesDetalle.Controls.AddRange(new Control[] { btnModificarDatos, btnVerMapa });

            var panelDetalle = new Panel { Dock = DockStyle.Fill };
            panelDetalle.Controls.Add(tablaDetalle);
            panelDetalle.Controls.Add(lblSinSeleccion);
            panelDetalle.Controls.Add(botonesDetalle);

            // Historial de cambios de estado
            gridHistorial = CrearGrilla();
            gridHistorial.Columns.Add("estado", "Estado");
            gridHistorial.Columns.Add("desde", "Desde");
            gridHistorial.Columns.Add("usuario", "Usuario");
            gridHistorial.SelectionChanged += (_, _) => gridHistorial.ClearSelection();

            // Resultado de la revisión
            comboAccion = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Margin = new Padding(3, 6, 10, 3) };
            comboAccion.Items.AddRange(GestorRegistrarResultado.Acciones);
            comboAccion.SelectedIndex = 0;
            btnRegistrar = new Button { Text = "Registrar resultado", AutoSize = true, Padding = new Padding(10, 3, 10, 3), Font = new Font(Font, FontStyle.Bold) };
            btnRegistrar.Click += (_, _) => tomarAccionConEvento();
            var filaAccion = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, Padding = new Padding(2, 10, 2, 4) };
            filaAccion.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaAccion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            filaAccion.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filaAccion.Controls.Add(new Label { Text = "Acción:", AutoSize = true, Margin = new Padding(3, 9, 3, 3) }, 0, 0);
            filaAccion.Controls.Add(comboAccion, 1, 0);
            filaAccion.Controls.Add(btnRegistrar, 2, 0);

            columna.Controls.Add(Agrupar("Detalle del evento", panelDetalle), 0, 0);
            columna.Controls.Add(Agrupar("Historial de estados", gridHistorial), 0, 1);
            columna.Controls.Add(Agrupar("Resultado de la revisión", filaAccion), 0, 2);
            return columna;
        }

        private static GroupBox Agrupar(string titulo, Control contenido)
        {
            var grupo = new GroupBox { Text = titulo, Dock = DockStyle.Fill, Padding = new Padding(8, 6, 8, 8), Margin = new Padding(0, 4, 0, 4) };
            contenido.Dock = DockStyle.Fill;
            grupo.Controls.Add(contenido);
            return grupo;
        }

        private static DataGridView CrearGrilla()
        {
            var grid = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 232, 235),
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                EnableHeadersVisualStyles = false,
                ShowCellToolTips = false,
                StandardTab = true,
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = ColorSecundario;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 249, 250);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(2, 5, 2, 5);
            grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            grid.RowTemplate.Height = 28;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(214, 230, 245);
            grid.DefaultCellStyle.SelectionForeColor = ColorTexto;
            return grid;
        }

        // ---------- Flujo del caso de uso ----------

        public void seleccionarRegistroResultadoRevisionManual()
        {
            eventos = gestor.tomarRegistroResultadoRevisionManual();
            mostrarEventosOrdenados();
            mostrarDatosSismicos();
            lblPendientes.Text = $"{eventos.Count} evento(s) pendiente(s) de revisión";
        }

        public void mostrarEventosOrdenados()
        {
            cargandoGrilla = true;
            gridEventos.Rows.Clear();
            foreach (var ev in eventos)
            {
                int i = gridEventos.Rows.Add(
                    ev.getFechaHoraOcurrencia().ToString("dd/MM/yyyy HH:mm"),
                    ev.getValorMagnitud().ToString("0.0", CultureInfo.CurrentCulture),
                    ev.getClasificacion().getNombre(),
                    $"{ev.getLatitudEpicentro():0.00}, {ev.getLongitudEpicentro():0.00}",
                    ev.obtenerEstadoActual()?.getDescripcion());
                gridEventos.Rows[i].Tag = ev;
            }
            gridEventos.ClearSelection();
            cargandoGrilla = false;
        }

        private void gridEventos_SelectionChanged(object? sender, EventArgs e)
        {
            if (cargandoGrilla || gridEventos.SelectedRows.Count == 0) return;
            tomarSeleccionEvento((EventoSismico)gridEventos.SelectedRows[0].Tag!);
        }

        public void tomarSeleccionEvento(EventoSismico evento)
        {
            gestor.tomarSeleccionEvento(evento);
            actualizarEstadosEnGrilla();
            mostrarDatosSismicos();
            mostrarMensaje("Evento bloqueado para su revisión.", ColorTexto);
        }

        private void actualizarEstadosEnGrilla()
        {
            foreach (DataGridViewRow fila in gridEventos.Rows)
            {
                var ev = (EventoSismico)fila.Tag!;
                fila.Cells["estado"].Value = ev.obtenerEstadoActual()?.getDescripcion();
                fila.Cells["magnitud"].Value = ev.getValorMagnitud().ToString("0.0", CultureInfo.CurrentCulture);
            }
        }

        public void mostrarDatosSismicos()
        {
            var evento = gestor.getEventoSeleccionado();
            bool hayEvento = evento != null;

            tablaDetalle.Visible = hayEvento;
            lblSinSeleccion.Visible = !hayEvento;
            btnModificarDatos.Enabled = btnVerMapa.Enabled = btnRegistrar.Enabled = comboAccion.Enabled = hayEvento;
            gridSeries.Rows.Clear();
            gridHistorial.Rows.Clear();
            if (evento == null) return;

            var magnitud = evento.getMagnitudRichter();
            valoresDetalle["Fecha y hora"].Text = evento.getFechaHoraOcurrencia().ToString("dd/MM/yyyy HH:mm:ss");
            valoresDetalle["Magnitud"].Text = $"{magnitud.getNumero():0.0}  ({magnitud.getDescripcion()})";
            valoresDetalle["Clasificación"].Text = evento.getClasificacion().getNombre();
            valoresDetalle["Epicentro"].Text = $"{evento.getLatitudEpicentro():0.00}, {evento.getLongitudEpicentro():0.00}  -  {evento.getProfundidadEpicentro():0} km";
            valoresDetalle["Hipocentro"].Text = $"{evento.getLatitudHipocentro():0.00}, {evento.getLongitudHipocentro():0.00}  -  {evento.getProfundidadHipocentro():0} km";
            valoresDetalle["Alcance"].Text = evento.getAlcance()?.getNombre() ?? "Sin cargar";
            valoresDetalle["Origen"].Text = evento.getOrigenDeGeneracion()?.getNombre() ?? "Sin cargar";
            valoresDetalle["Estado"].Text = evento.obtenerEstadoActual()?.getDescripcion();
            valoresDetalle["Alcance"].ForeColor = evento.getAlcance() == null ? ColorError : ColorTexto;
            valoresDetalle["Origen"].ForeColor = evento.getOrigenDeGeneracion() == null ? ColorError : ColorTexto;

            foreach (var serie in gestor.buscarSeriesTemporales())
            {
                double velocidadMax = serie.obtenerMuestras()
                    .SelectMany(m => m.obtenerDetallesMuestra())
                    .Where(d => d.getTipoDato().getDenominacion().StartsWith("Velocidad"))
                    .Select(d => d.getDatos())
                    .DefaultIfEmpty(0)
                    .Max();
                gridSeries.Rows.Add(
                    $"{serie.obtenerCodigoEstacion()} - {serie.obtenerNombreEstacion()}",
                    serie.obtenerNombreSismografo(),
                    serie.obtenerMuestras().Count,
                    velocidadMax.ToString("0.00", CultureInfo.CurrentCulture),
                    serie.getCondicionAlarma() ? "Sí" : "No");
            }
            gridSeries.ClearSelection();

            foreach (var cambio in evento.getCambiosDeEstado().Reverse())
            {
                gridHistorial.Rows.Add(
                    cambio.getEstado().getDescripcion(),
                    cambio.getFechaHoraInicio().ToString("dd/MM HH:mm:ss"),
                    cambio.getUsuario().getUsuario());
            }
        }

        private void btnModificarDatos_Click(object? sender, EventArgs e)
        {
            var evento = gestor.getEventoSeleccionado();
            if (evento == null) return;

            using var dialogo = new DialogoModificarEvento(
                evento.getAlcance()?.getNombre(), evento.getValorMagnitud(), evento.getOrigenDeGeneracion()?.getNombre());
            if (dialogo.ShowDialog(this) != DialogResult.OK) return;

            gestor.modificarDatosEvento(dialogo.Alcance, dialogo.Magnitud, dialogo.Origen);
            actualizarEstadosEnGrilla();
            mostrarDatosSismicos();
            mostrarMensaje("Datos del evento actualizados.", ColorExito);
        }

        public void mostrarOpcionVisualizarMapa()
        {
            var evento = gestor.getEventoSeleccionado();
            if (evento == null) return;

            var respuesta = MessageBox.Show(this,
                "Se abrirá el mapa con la ubicación del epicentro en el navegador. ¿Continuar?",
                "Visualizar mapa", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (respuesta != DialogResult.OK) return;

            string lat = evento.getLatitudEpicentro().ToString(CultureInfo.InvariantCulture);
            string lon = evento.getLongitudEpicentro().ToString(CultureInfo.InvariantCulture);
            string url = $"https://www.openstreetmap.org/?mlat={lat}&mlon={lon}#map=8/{lat}/{lon}";
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                mostrarMensaje($"No se pudo abrir el navegador: {ex.Message}", ColorError);
            }
        }

        public void tomarAccionConEvento()
        {
            var resultado = gestor.tomarAccionConEvento(comboAccion.SelectedItem as string);
            if (!resultado.Exito)
            {
                mostrarMensaje(resultado.Mensaje, ColorError);
                return;
            }

            seleccionarRegistroResultadoRevisionManual();
            comboAccion.SelectedIndex = 0;
            mostrarMensaje(resultado.Mensaje, ColorExito);
        }

        private void mostrarMensaje(string texto, Color color)
        {
            lblEstado.Text = texto;
            lblEstado.ForeColor = color;
        }
    }
}
