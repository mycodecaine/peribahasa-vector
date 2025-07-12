using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.PeribahasaVector.Application.ViewModels
{
    public record PeribahasaViewModel
    (
        string Teks,
        string Maksud,
        string TeksTranslation,
        string MaksudTranslation,
        string Context,
        string Source
    );
   
}
