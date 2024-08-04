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
There can only be one surplus operator, but it can be in any order (e.g. `a, ~b, c`).
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

If a method is redefined, the original method can be called with the `origin` method.
```
sub say(message)
  log(message)
end
sub say(message)
  log("I was here")
  origin
end
```

Box methods can be defined without the `eval` method.
```
sub cat.say(message)
end
```

Arguments can be passed by name.
```
cat.say(message = "meow")
```

Holo's type annotations are designed for simple type safety. Generic arguments are too complex for Holo.
However, methods can return generic values by annotating the return type as the name of an argument or `self`.
```
sub log_get(value:box):value
  log value
  return value
end
five := log_get(5) # five:int
```
```
animal := {
  sub deep_fake:self
    return self.new
  end
}
cat := {
  include animal
}
fake_cat := cat.deep_fake # fake_cat:cat
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
[get] (variable) - return this variable if method not found
[set] (variable) - set this variable if method not found
[private] (method) - warn if called outside of box / derived box
[abstract] (variable, method) - if variable, warn if `new` called; if method, warn if called and warn if not overridden in derived box
[static] (variable, method) - warn if accessed from derived box (instances are derived)
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
0 % 1               # 0.`%`(1)
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
name %= value
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
  
  [get] player := new(1)
  [get] animal := new
}
log(entity_type.player.name) # "player"
log(entity_type.animal.value) # 2
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

### Standard Library

#### box

Every box derives from box, even if not in the `components` table.
- `stringify():str` - returns "box"
- `new():self` - instantiates a new box with this box as a component
- `components():table` - returns the boxes included in this box
- `variables():table` - returns a table of variable names
- `methods():table` - returns a table of method names
- `eval(code:method):box` - executes the method in the box context
- `hash_code():int` - returns a lookup number
- `eval(code:str):box` - parses and executes the code in the box context
- `==(other:box):bool` - returns true if both boxes have the same reference
- `!=(other:box):bool` - calls `==` and returns the opposite
- `<=>(other:box):bool` - calls comparison operators (may not be needed, only include if required for table lookups)

#### global

The global box methods can be called from anywhere. They have a higher precedence than methods in `self`.
- `log(~messages):null` - logs each message to the standard output
- `warn(~messages):null` - logs each warning message to the standard output
- `throw(message:box):null` - creates an exception and throws it
- `throw(exception1:exception):null` - throws the exception
- `input():str` - reads and returns a line of user input
- `wait(duration:num = 0.001):dec` - yields for the duration in seconds and returns the exact amount of time waited
- `local_variables():table` - returns a table of variable names
- `rand(range1:range):num` - calls `random.rand` and returns a random number in the range
- `rand(max:num):max` - calls `random.rand` and returns a random number in the range
- `exit(code:int = 0):null` - exits the application with the given exit code
- `quit(code:int = 0):null` - calls `exit()`
- `holo_version():str` - returns the Holo language version
- `holo_copyright():str` - returns the Holo language copyrights

#### boolean (bool)

Has two instances: `true` and `false`.
- `stringify():str` - returns "true" if equals `true`, otherwise "false"

#### string (str) (includes sequence)

An immutable sequence of characters.
- `stringify():str` - returns self
- `count():int` - returns the number of characters
- `count(sequence:str):int` - returns the number of times the sequence appears
- `length():int` - calls `count()`
- `characters():table` - gets a table of characters in the string
- `trim(predicate:method = null):str` - removes characters matching a predicate from the start and end of the string (defaults to whitespace)
- `trim_start(predicate:method = null):str` - removes characters matching a predicate from the start of the string (defaults to whitespace)
- `trim_start(sequence:str):str` - removes the sequence from the start of the string
- `trim_end(predicate:method = null):str` - removes characters matching a predicate from the end of the string (defaults to whitespace)
- `trim_end(sequence:str):str` - removes the sequence from the end of the string
- `to_case(case:string_case_type):str` - converts the string to PascalCase, lowerCamelCase, snake_case, kebab-case, flatcase, Title Case, Sentence case
- `to_upper():str` - converts letters in the string to uppercase
- `to_lower():str` - converts letters in the string to lowercase
- `replace(find:str, with:str, limit:int? = null):str` - replaces each appearance of a sequence with another sequence up to limit times
- `+(other:box):str` - concatenates the string and other.stringify
- `*(count:int):str` - repeats the string count times
- `==(other:box):bool` - returns true if other is an equal string
- `<(other:box):bool` - returns true if other is a string preceding alphabetically
- `<=(other:box):bool` - returns true if other is a string equal or preceding alphabetically
- `>=(other:box):bool` - returns true if other is a string succeeding alphabetically
- `>(other:box):bool` - returns true if other is a string equal or succeeding alphabetically

#### number (num)

The base component for integers and decimals.
- `stringify():str` - returns "number"

#### integer (int)

A signed whole number with arbitrary size and precision.
- `stringify():str` - returns the integer as a string
- `parse(str1:str):int` - converts the string to an integer
- `parse_or_null(str1:str?):int?` - converts the string to an integer or returns null

#### decimal (dec)

A signed base-10 fractional number with arbitrary size and precision.
- `stringify():str` - returns the decimal as a string
- `parse(str1:str):dec` - converts the string to a decimal
- `parse_or_null(str1:str?):dec?` - converts the string to a decimal or returns null

#### iterator

Gets each item in a sequence.
- `stringify():str` - returns "iterator"
- `current():box` - returns the current item in the sequence
- `move_next():bool` - changes current to the next item in the sequence

#### sequence

An iterable, deferred sequence of items.
- `stringify():str` - concatenates the sequence to a string enclosed in square brackets
- `each():iterator` - returns an iterator for each item
- `to_table():table` - adds each item to a table
- `add(item:box):sequence` - adds an item to the end of the sequence
- `add_each(items:sequence):sequence` - adds each item to the end of the sequence
- `prepend(item:box):sequence` - adds an item to the start of the sequence
- `prepend_each(items:sequence):sequence` - adds each item to the start of the sequence
- `all(predicate:method):bool` - returns true if all items match the predicate
- `any(predicate:method):bool` - returns true if any item matches the predicate
- `any():bool` - returns true if the sequence has at least one item
- `count():int` - returns the number of items
- `count(sequence:str):int` - returns the number of times the sequence appears in self
- `length():int` - calls `count()`
- `concat(separator:str = "", stringify:method = null):str` - adds each item to a string by calling stringify
- `remove(item:box, limit:int? = null):sequence` - removes each appearance of the item up to limit times
- `remove_duplicates():sequence` - removes duplicate items
- `remove_where(predicate:method):sequence` - removes items matching a predicate
- `first(predicate:method):box` - returns the first item matching the predicate
- `first_or_null(predicate:method):box?` - returns the first item matching the predicate or null
- `first():box` - returns the first item
- `first_or_null():box` - returns the first item or null
- `last(predicate:method):box` - returns the last item matching the predicate
- `last_or_null(predicate:method):box?` - returns the last item matching the predicate or null
- `last():box` - returns the last item
- `last_or_null():box` - returns the last item or null
- `average(type:average_type = average_type.mean):num` - returns the average value of the sequence of numbers using mean, median, mode, or range
- `max(value:method = null):num` - gets the biggest value in the sequence of numbers
- `min(value:method = null):num` - gets the smallest value in the sequence of numbers
- `sort(comparer:method = null):sequence` - sorts the sequence into an order using the comparer
- `reverse():sequence` - reverses the order of the sequence
- `clone():sequence` - shallow-copies the sequence into another sequence

#### table (includes sequence)

An sequence of key-value pairs.
- `stringify():str` - concatenates the key-value pairs to a string enclosed in square brackets