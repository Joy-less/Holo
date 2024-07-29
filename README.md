# Holo

Holo is a unique scripting language inspired by Ruby and Lua. It aims for:
- Simplicity (keep it simple)
- Expressiveness (code your way)
- Embeddability (with .NET)

## Design Proposal

### Orientation

Holo uses "boxes" which serve as templates and instances.
Like Ruby, boxes expose methods, not variables.
Like Lua, there is no static/instance distinction.
There are no value types.

### Statements

Like Lua, code is separated into statements and expressions.
If there's only one expression (e.g. in a method body) it may be parsed as a statement.
Like Ruby, newlines are significant.
Like Lua, spaces/tabs are insignificant.
Assignment is not a valid expression.

### Variables

Variables are declared with a type and assigned without a type.
```
cats:int = 3
cats = 4
dogs := 2
```

Variable identifiers can be anything if wrapped in backticks.
```
`~#:@!` := 5
```

### Scopes

Code is organised into scopes which begin with `do` and end with `end`.
```
if true do
  log("hi")
end
```

`do` is optional if there's a keyword and a newline.
```
while true
  log("hi")
end
```

### Numbers

Holo has two number types: integers and decimals. Both are signed and arbitrarily large and precise (like Ruby).
```
int1 := 3
dec1 := 6.2
```

### Strings

Strings are immutable and can span multiple lines.
Double-quotes parse escape sequences whereas single-quotes ignore them (like Ruby).
```
string1 := "formatted"
string2 := 'not formatted'
```

Triple quotes trim the whitespace to the left of the closing quotes.
```
string1 := """
    formatted
    """
string2 := '''
    not formatted
    '''
```

Escape sequences start with a backslash. Any unrecognised escape sequence is a syntax error.
```
'single quote: \''
"double quote: \""
"interpolation: \{expression}"
"backslash: \\"
"newline: \n"
"carriage return: \r"
"tab: \t"
"backspace: \b"
"alert: \a"
```

### Ranges

Ranges can be created with `min to max (step value)`.
They are inclusive as Holo uses one-based indexing.
```
numbers:range = 1 to 10
odd_numbers:range = 1 to 10 step 2
```

### Boxes

Boxes are objects created with curly brackets.
Rather than inheriting from a base type, they include multiple components.

```
cat := {
  include animal
  include pathfinding

  sub meow
    log("nya")
  end
}
```

To instantiate a box, call the `new` method which clones the box and calls the `init` method.
```
cat := {
  name:str

  sub init(name:str)
    self.name = name
    log("new cat")
  end
}

tama = cat.new("Tama")
```

### Methods

Methods run under a context (`self`).

Methods are called by name. Brackets are optional.
```
cat.meow()
cat.meow
cat.eval(sub()
    meow
end)
```

Methods are declared with `sub name`. Brackets are mandatory if they have arguments.
```
sub meow()
  log("nya")
end
meow
```

Anonymous methods are declared with `sub` and wrapped in a method box. Brackets are mandatory.
```
meow := sub()
  log("nya")
end
meow.call
```

Multiple overloads for a method are allowed.
```
sub say(message:string)
  log(message)
end
sub say(message:int)
  log(message)
end

method("say").call # Call the best overload
method("say").overloads.first.call # Call the first overload
```

Surplus arguments can be caught with the surplus operator.
```
sub say(~things)
  for thing in things
    log(thing)
  end
end

one, ~two_three = [1, 2, 3]
```

Assignments on a box are translated to method calls (like Ruby).
```
cat := {
  sub set_name(name:str)
    # ...
  end
}
cat.name = "Robbie"
```

Missing methods can be caught with `missing` and `set_missing` methods.
```
cat := {
  sub missing(method)
    # ...
  end
  sub set_missing(method, value)
    # ...
  end
}
cat.length
cat.length = 5
```

If a method is redefined, the original method can be called with `origin`.
```
sub say(message)
  log(message)
end
sub say(message)
  log("I was here")
  origin
end
```

### Tables

Tables are a type of box that store key-value pairs.
If the key is omitted, they use one-based indexing (like Lua).
They can be created with square brackets.
```
nicknames := [
  "Kirito" = "Black Swordsman",
  "Asuna" = "Lightning Flash",
]

for nickname, name in nicknames
  log("\{name} a.k.a. \{nickname}")
end
```

Tables can be joined using the surplus operator.
```
joined = [~table1, ~table2]
```

### Attributes

Each box has a table of attributes for its variables and a table of attributes for its methods.
Attributes can be added with square brackets before a method or variable declaration.
They omit the `_attribute` and `.new` (like C#).
```
[summary("Classified.")]
sub get_nuclear_launch_codes()
  # ...
end

method_attributes.get("get_nuclear_launch_codes").get(1)
```

Core attributes:
```
[private] (method) - warn if called outside of box / derived box
[abstract] (variable, method) - if variable, warn if `new` called; if method, warn if not overridden in derived box
[static] (variable, method) - warn if accessed from derived box (including instances)
[summary(message:string)] (variable, method) - description for intellisense
[deprecated(message:string = null)] (variable, method) - warn if used
```

### Comments

Comments are ignored by the parser. There are no "magic" comments or documentation comments.

Line comments start with a hashtag.
```
# line comment
```

Block comments start with multiple hashtags and end with the same number of hashtags.
```
## block comment ##

#####
block comment
#####
```

### Fixed operators

Logic operators compare boolean values.
```
true and false
true or false
true xor false
not true
```

Null is handled with null propagation.
```
box?.value
box ?? value
box ??= value
```

### Method operators

These operators are shorthand for method calls.
```
0 == 1              # 0.`==`(1)
0 != 1              # 0.`!=`(1)
0 > 1               # 0.`>`(1)
0 < 1               # 0.`<`(1)
0 >= 1              # 0.`>=`(1)
0 <= 1              # 0.`<=`(1)
0 + 1               # 0.`+`(1)
0 - 1               # 0.`-`(1)
0 * 1               # 0.`*`(1)
0 / 1               # 0.`/`(1)
0 // 1              # 0.`/`(1).truncate()
0 ^ 1               # 0.`^`(1)
0 in [1, 2, 3]      # [1, 2, 3].contains(0)
0 not in [1, 2, 3]  # not [1, 2, 3].contains(0)
```

These assignment operators are shorthand for applying method operators to the current value.
```
name += value
name -= value
name *= value
name /= value
name ^= value
```

### Iteration

There are two types of iteration.

While loops repeat every time a condition is not `false` or `null`.
```
while true
  # ...
end
```

For loops repeat using an iterator returned from the `each` method.
```
for i in 1 to 10
  log i
end
```

Loops can be ended early with `break`.
```
while true
  break
end
```

The current iteration can be skipped with `next`, which moves to the end of the loop body.
```
for i in 1 to 10
  next
end
```

### Selection

There are two types of selection.

If statements run the first branch whose condition is not `false` or `null`.
```
if access_level == "noob"
  log "denied"
elseif access_level == "normie"
  log "denied"
else
  log "come on in"
end
```

Cases run the first branch matching the subject.
Valid when branches: `when matches`, `when == match`, `when != match`, `when in match`, `when not in match`, `when > match`, `when < match`, `when >= match`, `when <= match`.
```
case input
when 0, 1
  log("zero or one")
when 2
  log("two")
when in [3, 4, 5]
  log("three, four or five")
when > 5
  log("big")
else
  log("negative?")
end
```

### Exceptions

Most languages use `try`/`catch` blocks for exception handling.
Holo simplifies this with `try`/`else` where `try` always catches the exception.
```
try
  throw "exception"
else ex
  log(ex.message)
ensure
  log("always runs")
end
```

### Enums

Languages like Ruby use strings as enums. These suffer from typos, and don't easily support integers (useful for storing enums in files).
Languages like GDScript use integers as enums. These suffer from poor debugging readability.

Holo supports enums as a type of box containing a string and a number.
```
entity_type := {
  include enum
  
  player := new(1)
  animal := new
}
log(entity_type.player.to_str) # "player"
log(entity_type.animal.to_int) # 2
```

### Events

Events can be awaited and connected easily.
```
on_fire := event.new()
on_fire.invoke()
on_fire.wait()
on_fire.listen(sub()
  # ...
end)
```

### Race Conditions

Holo uses collaborative multithreading with actors (like Lua/Luau). As such, race conditions are usually not a problem.
```
log(1)
log(2) # actor is locked between log(1) and log(2)
wait()
log(3)
```

If you need to lock over asynchronous methods, you can use a mutex, which limits the number of calls that can run at once.
```
mutex1 := mutex.new(1)
mutex1.run(sub()
  # ...
end)
```

### Goto

It could be useful.
```
goto hello
label hello
```