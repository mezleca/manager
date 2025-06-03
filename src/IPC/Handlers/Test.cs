using System;
using MessagePack;

namespace IPC
{

    [MessagePackObject]
    class TestRequest
    {
        [Key("content")]
        public required string? Content { get; set; } 
    }

    [MessagePackObject]
    class TestResponse
    {
        [Key("success")]
        public bool Success { get; set; }
        [Key("content")]
        public string Content { get; set; } 
    }
}