﻿namespace QualityControlApp.ViewModels
{
    public class QuestionContentUpdateVM
    {
        public Guid Id { get; set; }
        public int? Score { get; set; }
        public string? Inspect { get; set; }
        public string? Nots { get; set; }
        public int? Level { get; set; }
    }
}
