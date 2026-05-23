using System;
using System.Collections.Generic;

namespace MyExam.Backend.Models.DbMysqlModels;

public partial class PizzaRendelesTetelek
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public string? Name { get; set; }

    public int? Amount { get; set; }

    public int? Price { get; set; }
}
