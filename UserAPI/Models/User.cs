using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DefaultValue(false)]
    public bool IsAdmin { get; set; }

    [Required]
    [Range(1, 150)] // Adjust the range based on your application's requirements
    public int Age { get; set; }

    [Required]
    public List<string> Hobbies { get; set; } = new List<string>();
}
 