using Milongas.Extractor.Models;

namespace Milongas.App;

public partial class FormDetalleMilonga : Form
{
    private readonly Milonga milonga;

    public FormDetalleMilonga(
        Milonga milonga)
    {
        InitializeComponent();

        this.milonga = milonga;

        Text = milonga.Nombre;
    }
}