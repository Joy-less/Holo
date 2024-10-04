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

Both methods and variables are public by default.
Methods have higher priority in dot notation, whereas variables have higher priority when named.

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

Methods run under a context accessible with `self`.

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
There can only be one surplus operator, but it can be in any order (e.g. `a, ..b, c`).
```
sub say(..things)
  for thing in things
    log(thing)
  end
end

one, ..two_three = [1, 2, 3]
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

### Type Annotations

Type annotations are treated as methods, which are called every time they are used.

This allows you to pass them as variables:
```
sub increment(value:num):value
  big_value:value = value + 1
  return big_value
end
```

The `(of ..types)` operator (taken from Visual Basic) can be used on boxes. If not overloaded, the arguments can be retrieved with `types()` or `types(key)`.
```
toy_box := {
  contents:types(1)

  # example overload
  sub of(types:table):null
    origin(types)
  end
}

toy_box1 := toy_box(of int).new()
toy_box1.contents = "ball" # error
```

Example using `self` as a type:
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

Examples of typing tables:
```
items := ["red", "blue", "green"](of int, string)
```
```
items:table(of int, string) := ["red", "blue", "green"]
```

### Casts

When a box is assigned to a variable of a different type, it is cast to that type.
```
health:decimal = 100
```

It does this using the `cast(to_type)` method.
```
sub cast(to_type)
  if to_type is decimal
    return # ...
  else
    return # ...
  end
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

for nickname in nicknames
  log("\{nickname.key} a.k.a. \{nickname.value}")
end
```

Tables can be joined using the surplus operator.
```
joined = [..table1, ..table2]
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
[private] (variable, method) - warn if accessed outside of box / derived box
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
0 not_in [1, 2, 3]  # not [1, 2, 3].contains(0)
0 is integer        # 0.includes(integer)
0 is_not integer     # not 0.includes(integer)
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
Valid when branches: `when matches`, `when == match`, `when != match`, `when in match`, `when not_in match`, `when > match`, `when < match`, `when >= match`, `when <= match`.
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
Holo attempts to simplify this with `try`/`else`, where `try` always catches the exception.
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

Ruby uses strings (symbols) as enums. It suffers from typos and doesn't support integer casts.

GDScript uses integers as enums. It suffers from poor debugging readability.

Holo uses enums as a type of box that can be created with `enum` or `enum.new`. They contain a name:string and a value:number.
```
entity_type := enum(
  player = 1,
  animal = 2,
)

log(entity_type.get("player").name) # "player"
log(entity_type.get("animal").value) # 2

log (entity_type.animal.name) # calls missing method; same as entity_type.get("animal").name
```

### Events

Events can be awaited and connected easily.
```
on_fire := event.new()
on_fire.invoke()
on_fire.wait()
on_fire.hook(sub()
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

Every box includes box, even if not in the `components` table.
- `stringify():str` - returns "box"
- `type():str` - returns "box"
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

#### null

A box representing no box. Type annotations exclude null unless they have a question mark (`?`).
- `stringify():str` - returns "null"
- `type():str` - returns "null"

#### global

The global box methods can be called from anywhere. They have a higher precedence than methods in `self`.
- `type():str` - returns "global"
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
- `type():str` - returns "boolean"

#### string (str) (includes sequence)

An immutable sequence of characters.
- `stringify():str` - returns self
- `type():str` - returns "string"
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
- `insert(sequence:str, position:int):str` - inserts the sequence at the position
- `split(separator:str):table` - separates the string into a table of strings
- `split():table` - separates the string by whitespace into a table of strings
- `remove_range(between:range):str` - removes characters within the range
- `+(other:box):str` - concatenates the string and other.stringify
- `*(count:int):str` - repeats the string count times
- `==(other:box):bool` - returns true if other is an equal string
- `<(other:box):bool` - returns true if other is a string preceding alphabetically
- `<=(other:box):bool` - returns true if other is a string equal or preceding alphabetically
- `>=(other:box):bool` - returns true if other is a string succeeding alphabetically
- `>(other:box):bool` - returns true if other is a string equal or succeeding alphabetically

#### [abstract] number (num)

The base component for integers and decimals.
- `stringify():str` - returns "number"
- `type():str` - returns "number"
- `to_dec():dec` - converts the number to a decimal
- `to_radians():num` - converts degrees to radians
- `to_rad():num` - calls `to_radians()`
- `to_degrees():num` - converts radians to degrees
- `to_deg():num` - calls `to_degrees()`
- `sqrt():num` - returns the square root of the number
- `cbrt():num` - returns the cube root of the number
- `lerp(to:num, weight:num):dec` - linearly interpolates the number
- `abs():num` - returns the positive value of the number
- `clamp(min, max):num` - returns min if < min, max if > max, otherwise self
- `floor():int` - returns the highest integer below the number
- `ceil():int` - returns the lowest integer above the number
- `truncate():int` - removes the decimal places of the number

#### integer (int)

A signed whole number with arbitrary size and precision.
- `stringify():str` - returns the integer as a string
- `type():str` - returns "integer"
- `parse(str1:str):int` - converts the string to an integer
- `parse_or_null(str1:str?):int?` - converts the string to an integer or returns null

#### decimal (dec)

A signed base-10 fractional number with arbitrary size and precision.
- `stringify():str` - returns the decimal as a string
- `type():str` - returns "decimal"
- `parse(str1:str):dec` - converts the string to a decimal
- `parse_or_null(str1:str?):dec?` - converts the string to a decimal or returns null

#### iterator

Gets each item in a sequence.
- `stringify():str` - returns "iterator"
- `type():str` - returns "iterator"
- `current():table` - returns the current items in the sequence
- `move_next():bool` - tries to increment the sequence position

#### sequence

An iterable, deferred sequence of items.
- `stringify():str` - concatenates the sequence to a string separated by commas enclosed in square brackets
- `type():str` - returns "sequence"
- `each():iterator` - returns an iterator for each item
- `to_table():table` - adds each item to a new table
- `append(item:box):sequence` - adds an item to the end of the sequence
- `append_each(items:sequence):sequence` - adds each item to the end of the sequence
- `prepend(item:box):sequence` - adds an item to the start of the sequence
- `prepend_each(items:sequence):sequence` - adds each item to the start of the sequence
- `all(predicate:method):bool` - returns true if all items match the predicate
- `any(predicate:method):bool` - returns true if any item matches the predicate
- `any():bool` - returns true if the sequence has at least one item
- `count():int` - returns the number of items
- `count(item:box):int` - returns the number of times the item appears
- `length():int` - calls `count()`
- `contains(item:box):bool` - returns true if sequence contains item
- `concat(separator:str = "", stringify:method = null):str` - adds each item to a string by calling stringify
- `remove(item:box, limit:int? = null):sequence` - removes each appearance of the item up to limit times
- `remove_where(predicate:method, limit:int? = null):sequence` - removes items matching a predicate up to limit times
- `remove_first(count:int = 1)` - removes the first count items
- `remove_last(count:int = 1)` - removes the last count items
- `remove_duplicates():sequence` - removes duplicate items
- `clear():sequence` - removes all items
- `first(predicate:method):box` - returns the first item matching the predicate
- `first_or_null(predicate:method):box?` - returns the first item matching the predicate or null
- `first():box` - returns the first item
- `first_or_null():box` - returns the first item or null
- `last(predicate:method):box` - returns the last item matching the predicate
- `last_or_null(predicate:method):box?` - returns the last item matching the predicate or null
- `last():box` - returns the last item
- `last_or_null():box` - returns the last item or null
- `max(value:method = null):num` - gets the biggest value in the sequence of numbers
- `min(value:method = null):num` - gets the smallest value in the sequence of numbers
- `average(type:average_type = average_type.mean):num` - returns the average value of the sequence of numbers using mean, median, mode, or range
- `sum():num` - adds all items in the sequence of numbers
- `product():num` - multiplies all items in the sequence of numbers
- `sort(comparer:method = null):sequence` - sorts the sequence into an order using the comparer
- `reverse():sequence` - reverses the order of the sequence
- `clone():sequence` - shallow-copies the sequence into another sequence

#### table (includes sequence)

An sequence of key-value pairs.
- `type():str` - returns "table"
- `each():iterator` - returns an iterator for each [value, key]
- `add(value:box):table` - adds a value at the key one above the highest ordinal key
- `add_each(values:table):table` - adds each value at the keys one above the highest ordinal key
- `set(entry1:entry):table` - adds an entry to the table
- `set(key:box, value:box):table` - creates and adds an entry to the table
- `get(key:box):box` - finds a value from the key
- `get_or_null(key:box?):box?` - finds a value from the key or returns null
- `keys():table` - returns a table of keys
- `values():table` - returns a table of values
- `contains_key(key:box?):bool` - returns true if there's an entry with the given key
- `contains_value(value:box?):bool` - returns true if there's an entry with the given value
- `sample():entry` - returns a random entry
- `shuffle():entry` - randomises the keys
- `invert():table` - swaps the keys and values
- `weak():bool` - returns true if the values are weakly referenced
- `set_weak(value:bool):null` - references values weakly so they can be garbage collected

#### entry

A key-value pair in a table.
- `stringify():str` - returns (key + " = " + value)
- `type():str` - returns "entry"
- `key():box` - returns the key
- `value():box` - returns the value

#### range (includes sequence)

A range between two inclusive numbers.
- `type():str` - returns "range"
- `stringify():str` - returns (min + " to " + max + " step " + step)
- `new(min:num?, max:num?, step:num = 1)` - returns a new range
- `min():num?` - returns the minimum value
- `set_min(value:num?):null` - sets the minimum value
- `max():num?` - returns the maximum value
- `set_max(value:num?):null` - sets the maximum value
- `step():num` - returns the step value
- `set_step(value:num):null` - sets the step value

#### time

A date and time on planet Earth.
- `stringify():str` - returns the time formatted like "2024/08/04 15-46 +0000"
- `type():str` - returns "time"
- `new(year:num = 0, month:num = 0, day:num = 0, hour:num = 0, minute:num = 0, second:num = 0, offset:num = 0)` - returns a new time
- `now(local:bool = false):time` - returns a new time at the current time (UTC or local)
- `epoch(timestamp:num = 0):time` - returns a new time at timestamp seconds after the epoch (1970-01-01 00:00:00 +0)
- `timestamp():dec` - returns the number of seconds since the `epoch()`
- `year():dec` - returns the year component
- `set_year(value:num):dec` - sets the year component
- `month():dec` - returns the month component
- `set_month(value:num):dec` - sets the month component
- `day():dec` - returns the day component
- `set_day(value:num):dec` - sets the day component
- `hour():dec` - returns the hour component
- `set_hour(value:num):dec` - sets the hour component
- `minute():dec` - returns the minute component
- `set_minute(value:num):dec` - sets the minute component
- `second():dec` - returns the second component
- `set_second(value:num):dec` - sets the second component
- `offset():dec` - returns the offset component
- `set_offset(value:num):dec` - sets the offset component
- `total_seconds():dec` - calculates the total number of seconds since the epoch
- `total_milliseconds():dec` - divides `total_seconds()` by 1000

#### exception

An error or control code thrown up the call stack.
- `stringify():str` - returns (`message()` + "\n" + `strack_trace()`)
- `type():str` - returns "exception"
- `new(message:box? = null)` - returns an exception instance with the given message
- `message():box` - returns the exception message
- `set_message():str` - sets the exception message
- `stack_trace():table` - returns each line of the call stack (including external lines from C#)

#### weak_ref

Holds a reference to a box without preventing it from being garbage collected.
- `stringify():str` - returns (`message()` + "\n" + `strack_trace()`)
- `type():str` - returns "exception"
- `context():box` - returns the weakly referenced box
- `set_context(value:box):null` - sets the weakly referenced box
- `is_alive():bool` - returns true if the weak reference is still valid

#### thread

Runs code in the background collaboratively.
- `call(method1:method, arguments:table = []):thread` - calls a method in the thread
- `wait():` - waits for the thread to finish

#### canceller

Cancels a background task collaboratively.
- `cancel(delay:num = 0)` - calls `on_cancel()` after the delay
- `on_cancel():event` - an event to be invoked when cancelled
- `is_cancelled():bool` - returns true if cancelled
- `reset():null` - un-cancels the canceller for reuse
- `combine(other:canceller):canceller` - combines two cancellers into one

#### event

A signal to be awaited and listened to.
- `invoke(arguments:table = []):null` - calls each listener and waiter
- `listen(method1:method, limit:int? = null):null` - calls the method when the event is invoked up to limit times
- `wait():table` - waits for the event to be invoked

#### math

A collection of nerdy maths methods.
- `pi():dec` - returns many digits of pi (3.14...)
- `tau():dec` - returns many digits of tau (6.28...)
- `e():dec` - returns many digits of e (2.71...)
- `lerp(from:num, to:num, weight:num):dec` - calls `from.lerp(to, weight)`
- `sin`, `cos`, `tan`, `asin`, `acos`, `atan`, `atan2`, `sinh`, `cosh`, `tanh`, `asinh`, `acosh`, `atanh`, `exp`, `log`, `log10`, `log2`, `hypot` - who even knows what most of these do

#### file

Access files on the hard drive.
- `read(path:str):str` - opens and reads text from the file
- `read_bytes(path:str):table` - opens and reads a table of bytes from the file
- `write(path:str, value:str):null` - opens and writes text to the file
- `write_bytes(path:str, value:table):null` - opens and writes a table of bytes to the file
- `append(path:str, value:str):null` - opens and appends text to the file
- `append_bytes(path:str, value:table):null` - opens and appends a table of bytes to the file
- `delete(path:str):bool` - deletes the file
- `exists(path:str):bool` - returns true if the file exists
- `combine_path(a:str, b:str):str` - combines two paths into one
- `absolute_path(relative:str):str` - converts the relative path to an absolute path
- `relative_path(absolute:str):str` - converts the absolute path to a relative path
- `file_name(path:str, extension:bool = true):str` - returns the file name from the path (e.g. "C://Documents/neko.jpg" becomes "neko.jpg")
- `dir_path(path:str):str` - returns the directory path from the path (e.g. "C://Documents/neko.jpg" becomes "C://Documents")

#### random

Generate pseudo-random numbers.
- `seed():int` - returns the random seed
- `set_seed(value:int):null` - sets the random seed
- `int(min:num, max:num):int` - returns a random integer between min and max
- `int(max:num):int` - calls `int(1, max)`
- `int(range1:range):int` - returns a random integer in the range
- `dec(min:num, max:num):dec` - returns a random decimal between min and max
- `dec(range1:range):dec` - returns a random decimal in the range