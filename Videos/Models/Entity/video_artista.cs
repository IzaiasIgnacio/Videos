//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Videos.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class video_artista
    {
        public int id { get; set; }
        public int id_video { get; set; }
        public int id_artista { get; set; }
        public bool principal { get; set; }
    
        public virtual artista artista { get; set; }
        public virtual video video { get; set; }
    }
}