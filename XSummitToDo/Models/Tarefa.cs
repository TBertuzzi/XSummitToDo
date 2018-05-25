using System;
using Realms;

namespace XSummitToDo.Models
{
    public class Tarefa : RealmObject
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Titulo { get; set; }
       
        public string Descricao { get; set; }

        public byte[] Anexo { get; set; }

        public bool Concluido { get; set; }

        public bool Protegida { get; set; }

        public bool Agendada { get; set; }

    }
}
