/* Obfuzkator - (C) 2022 Premysl Fara */

using System.Text;


Console.WriteLine("Obfuzkator v1.0.0");

// ---

if (args.Length < 3)
{
    PrintUsage();
    
    return 1;
}

var cmd = args[0].ToLowerInvariant();
switch (cmd)
{
    case "o" or "obfuscate":
        cmd = "O";
        break;
    
    case "d" or "defuscate":
        cmd = "D";
        break;

    default:
    {
        Console.Error.WriteLine($"Unknown command '{cmd}'.");
        
        return 10;
    }
}

var inputFilePath = args[1];
if (File.Exists(inputFilePath) == false)
{
    Console.Error.WriteLine($"The '{inputFilePath}' input file does not exists.");
    
    return 10;
}

var outputFilePath = args[2];

// ---

var words = new[]
{
    "A:The", "B:Be", "C:To", "D:Of", "E:And", "F:A", "G:In", "H:That", "I:Have", "J:I", "K:It", "L:For", "M:Not", "N:On", "O:With", "P:He", "Q:As", "R:You", "S:Do", "T:At", "U:This", "V:But", "W:His", "X:By", "Y:From", "Z:They",
    "a:we", "b:say", "c:her", "d:she", "e:or", "f:an", "g:will", "h:my", "i:one", "j:all", "k:would", "l:there", "m:their", "n:what", "o:so", "p:up", "q:out", "r:if", "s:about", "t:who", "u:get", "v:which", "w:go", "x:me", "y:when", "z:make",
    "0:can", "1:like", "2:time", "3:no", "4:just", "5:him", "6:know", "7:take", "8:people", "9:into",
    "+:year", "/:your", "=:good",
};

// ---

try
{
    if (cmd == "O")
    {
        Console.WriteLine($"Obfuscating '{inputFilePath}' to '{outputFilePath}'...");
        
        File.WriteAllText(
            outputFilePath,
            Obfuscate(
                File.ReadAllBytes(inputFilePath)));
    }
    else if (cmd == "D")
    {
        Console.WriteLine($"Defuscating '{inputFilePath}' to '{outputFilePath}'...");
        
        File.WriteAllBytes(
            outputFilePath,
            Defuscate(
                File.ReadAllText(inputFilePath)));
    }
    else
    {
        Console.Error.WriteLine("What?");

        return 1024;
    }
    
    Console.WriteLine("DONE!");

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);

    return 10;
}


// var inputString = "Hello, World! ěščřžýáíé";
//
// var obfuscatedString = Obfuscate(Encoding.Unicode.GetBytes(inputString));
// var defuscatedBytes = Defuscate(obfuscatedString);
// var outputString = Encoding.Unicode.GetString(defuscatedBytes);


void PrintUsage()
{
    Console.WriteLine("Usage: obfuzkator command input-file-path output-file-path");
    Console.WriteLine("  command = o (obfuscate) or d (defuscate)");
}


string Obfuscate(byte[] bytes)
{
    if (bytes.Length == 0) throw new ArgumentException("Nothing to obfuscate!");

    var encodeMap = CreateEncodeMap(words);
    var base64 = System.Convert.ToBase64String(bytes);

    var sb = new StringBuilder(base64.Length * 5);
    var count = 0;
    foreach (var c in base64)
    {
        if (encodeMap!.ContainsKey(c) == false)
        {
            throw new Exception($"Unknown char '{c}' to encode!");
        }

        sb.Append(encodeMap[c]);
        
        count++;
        if (count > 16)
        {
            sb.AppendLine();
            count = 0;
        }
        else
        {
            sb.Append(' ');
        }
    }
    
    return sb.ToString();
}


byte[] Defuscate(string data)
{
    if (string.IsNullOrWhiteSpace(data)) throw new ArgumentException("Nothing to defuscate.");

    var decodeMap = CreateDecodeMap(words);
    
    var lines = data.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    var sb = new StringBuilder(data.Length);
    foreach (var line in lines)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        var lineSplit = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in lineSplit)
        {
            if (decodeMap.ContainsKey(word) == false)
            {
                throw new Exception($"Unknown word '{word}' to decode!");
            }

            sb.Append(decodeMap[word]);
        }
    }
    
    return System.Convert.FromBase64String(sb.ToString());
}


Dictionary<char, string> CreateEncodeMap(IEnumerable<string> wordsList)
{
    var encodeMap = new Dictionary<char, string>();
    
    foreach (var word in wordsList)
    {
        var split = word.Split(":");

        var c = split[0][0];
        var w = split[1];
    
        encodeMap.Add(c, w);
    }

    return encodeMap;
}


Dictionary<string, char> CreateDecodeMap(IEnumerable<string> wordsList)
{
    var decodeMap = new Dictionary<string, char>();

    foreach (var word in wordsList)
    {
        var split = word.Split(":");

        var c = split[0][0];
        var w = split[1];
    
        decodeMap.Add(w, c);
    }

    return decodeMap;
}


/*

https://cs.wikipedia.org/wiki/Base64
https://en.wikipedia.org/wiki/Most_common_words_in_English

A The
B Be
C To
D Of
E And
F A
G In
H That
I Have
J I
K It
L For
M Not
N On
O With
P He
Q As
R You
S Do
T At
U This
V But
W His
X By
Y From
Z They
a we
b say
c her
d she
e or
f an
g will
h my
i one
j all
k would
l there
m their
n what
o so
p up
q out
r if
s about
t who
u get
v which
w go
x me
y when
z make
0 can
1 like
2 time
3 no
4 just
5 him
6 know
7 take
8 people
9 into
+ year
/ your
= good

 */