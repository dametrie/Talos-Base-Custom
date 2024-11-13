
# Talos

Talos is a C# reverse engineering project to learn about tools and processes for obfuscation/deobfuscation.

Talos is a bot for the game [Dark Ages](https://en.wikipedia.org/wiki/Dark_Ages_(1999_video_game)) by Kru. The game can be found at www.darkages.com

The general principle is that a proxy server is established on a loobpack port that allows the user to intercept, modify, and send on packets to the game server. This allows the user to control many, if not all, aspects of the game (e.g., walking, casting spells, communicating with other players).

A well-known bot in the community called Zeus (formerly Accolade) was the target binary for this project. The binary was obfuscated using [ConfuserEx](https://github.com/yck1509/ConfuserEx), and potentially custom modules.

Deobfuscation was achieved using a variety of tools:\
[ConfuserEx-Unpacker](https://github.com/XenocodeRCE/ConfuserEx-Unpacker)\
[de4dot](https://github.com/de4dot/de4dot)\
[dnSpy](https://github.com/dnSpy/dnSpy)\
[DotPeek](https://www.jetbrains.com/decompiler/)

The end result was a reconstructed solution containing files with generic names like Class20.cs that contained methods and variables with generic names like:
```
internal class Class20
{
  internal string String_0 { get; set; }

  internal int Int32_0 { get; set; }

  internal int Int32_1 { get; set; }

  internal int Int32_2 => this.Int32_0 - this.Int32_1;

  internal Class20(string value)
  {
    this.String_0 = value;
    this.Int32_0 = 0;
    this.Int32_1 = 0;
  }
}
```

However, with a general knowledge of the underlying data types, packet structures, and examples from older bots, the codebase was slowly reconstructed by giving appropriate names to classes, methods, and variables.

While many of the smaller and less complex methods were easily transformed into human-readable code, the larger and more complex methods (e.g., pathfinding and casting logic) needed to be broken down and rewritten completely, as renaming variables was not sufficient to recreate the intended behavior of the bot. 

```
The deobfuscated codebase can be found in this repository under the name Zeus-deobfuscated.zip.
```
