using System;
using System.Collections.Generic;

namespace StoreAPIUXO.Models;

public partial class category
{
    public int categoryid { get; set; }

    public string? categoryname { get; set; }

    public int? categorystatus { get; set; }
}
