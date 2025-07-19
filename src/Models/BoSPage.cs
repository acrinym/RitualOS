using System;
using System.Collections.Generic;

namespace RitualOS.Models
{
    public class BoSPage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ParentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public List<BoSAttachment> Attachments { get; set; } = new();
        public List<BoSPage> Children { get; set; } = new();
    }

    public class BoSAttachment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FilePath { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // image, audio, journal, etc.
        public string? Description { get; set; }
    }
} 