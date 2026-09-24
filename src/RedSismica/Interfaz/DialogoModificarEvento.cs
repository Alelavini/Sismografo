using System;
using System.Drawing;
using System.Windows.Forms;

namespace RedSismica
{
    // Permite al analista corregir magnitud, alcance y origen antes de registrar el resultado
    public class DialogoModificarEvento : Form
    {
        private readonly ComboBox comboAlcance;
        private readonly ComboBox comboOrigen;
        private readonly NumericUpDown numMagnitud;

        public string Alcance => (string)comboAlcance.SelectedItem!;
        public string Origen => (string)comboOrigen.SelectedItem!;
        public double Magnitud => (double)numMagnitud.Value;

        public DialogoModificarEvento(string? alcance, double magnitud, string? origen)
        {
            Text = "Modificar datos del evento";
            Font = new Font("Segoe UI", 9.75f);
            AutoScaleMode = AutoScaleMode.Font;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = MinimizeBox = ShowInTaskbar = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(12);

            numMagnitud = new NumericUpDown { Minimum = 0.1m, Maximum = 10m, DecimalPlaces = 1, Increment = 0.1m, Width = 90 };
            numMagnitud.Value = Math.Clamp((decimal)Math.Round(magnitud, 1), numMagnitud.Minimum, numMagnitud.Maximum);

            comboAlcance = CrearCombo(AlcanceSismo.Nombres, alcance);
            comboOrigen = CrearCombo(OrigenDeGeneracion.Nombres, origen);

            var aceptar = new Button { Text = "Guardar", DialogResult = DialogResult.OK, AutoSize = true, MinimumSize = new Size(90, 0) };
            var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, AutoSize = true, MinimumSize = new Size(90, 0) };
            var botones = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Anchor = AnchorStyles.Right,
                Margin = new Padding(0, 14, 0, 0)
            };
            botones.Controls.AddRange(new Control[] { cancelar, aceptar });

            // Una única tabla con tamaño automático define el tamaño del diálogo
            var tabla = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Location = new Point(Padding.Left, Padding.Top)
            };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            AgregarFila(tabla, "Magnitud (Richter)", numMagnitud);
            AgregarFila(tabla, "Alcance", comboAlcance);
            AgregarFila(tabla, "Origen de generación", comboOrigen);
            tabla.Controls.Add(botones);
            tabla.SetColumnSpan(botones, 2);

            AcceptButton = aceptar;
            CancelButton = cancelar;
            Controls.Add(tabla);
        }

        private static ComboBox CrearCombo(string[] opciones, string? seleccionado)
        {
            var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
            combo.Items.AddRange(opciones);
            int indice = seleccionado == null ? -1 : Array.IndexOf(opciones, seleccionado);
            combo.SelectedIndex = indice >= 0 ? indice : 0;
            return combo;
        }

        private static void AgregarFila(TableLayoutPanel tabla, string etiqueta, Control control)
        {
            tabla.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(3, 8, 16, 3) });
            control.Margin = new Padding(3, 5, 3, 3);
            tabla.Controls.Add(control);
        }
    }
}
