# Holo

Holo is a scripting language inspired by Ruby, Lua and C#. It aims for:
- Simplicity (keep it simple)
- Expressiveness (code your way)
- Embeddability (with .NET)

## Examples

### FizzBuzz
```
sub fizzbuzz(n) do
  for i in 1 to n do
    if i % 3 == 0 and i % 5 == 0 do
      log "FizzBuzz"
    elseif i % 3 == 0 do
      log "Fizz"
    elseif i % 5 == 0 do
      log "Buzz"
    else do
      log i
    end
  end
end
```

### Fibonacci Sequence
```
sub fibonacci(n)
  if n in [0, 1] do return n end
  return fibonacci(n - 1) + fibonacci(n - 2)
end
log fibonacci(10)
```

## Design Proposal

### Orientation

Holo uses "boxes" which serve as templates and instances.
Boxes contain methods and variables, which are both public by default.
There are no value types.

### Statements

Code is separated into statements and expressions.
Newlines are significant but spaces/tabs are ignored.
Assignment is not a valid expression.

### Variables

Variables are declared with `var` and an optional type and value.
```
var cats:Int = 3
cats = 4
```

The `:=` operator infers the type by calling `class()` on the value.
```
var cats := 3
```

Variables default to `null`.
An exception is thrown if a non-nullable variable is used before being assigned or is assigned `null`.
Types can be made nullable with `?`.
```
var count:Int
log(count) # throws error
count = null # throws error
```

Variable identifiers can contain symbols if wrapped in backticks.
```
var `~#:@!` := 5
```

### Scopes

Code is organised into scopes which begin with `do` and end with `end`.
```
if true do
  log("hi")
end
```

You can omit `do` if there's a keyword and a newline.
```
while true
  log("hi")
end
```

### Numbers

Holo has two number types: integers and decimals. Both are signed and arbitrarily large and precise (like Ruby).
```
var int := 3
var dec := 6.2
```

### Strings

Strings are immutable and can span multiple lines.
Double-quotes parse escape sequences whereas single-quotes ignore them (like Ruby).
```
var string := "formatted"
var string2 := 'not formatted'
```

Triple quotes trim the whitespace to the left of the closing quotes.
```
var string := """
    formatted
    """
var string2 := '''
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
"binary: \0bInteger"
"hexadecimal: \0xInteger"
```

### Ranges

Ranges can be created with `min to max (step value)`.
They are inclusive as Holo uses one-based indexing.
```
var numbers:Range = 1 to 10
var odd_numbers:Range = 1 to 10 step 2
```

### Boxes

Boxes are objects created with curly brackets.
Rather than inheriting from a base type, they include multiple components.

Both methods and variables are public by default.
Methods have higher priority in dot notation, whereas variables have higher priority when named.

```
var Cat := {
  include animal
  include pathfinding

  sub meow()
    log("nya")
  end
}
```

To instantiate a box, call the `new` method which clones the box and calls the `init` method.
```
var Cat := {
  var name:Str

  sub init(name:Str)
    self.name = name
  end
}

var tama := Cat.new("Tama")
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

Methods are declared with `sub name()`. Brackets are mandatory.
```
sub meow()
  log("nya")
end
meow
```

Anonymous methods (stored in method boxes) are declared with `sub()`. Brackets are mandatory.
```
var meow = sub()
  log("nya")
end
meow.call
```

Multiple overloads for a method are allowed.
```
sub say(message:String)
  log(message)
end
sub say(message:Int)
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

var one, ..two_three = [1, 2, 3]
```

Assignments on a box are translated to method calls (similar to Ruby).
```
var Cat := {
  sub set_name(name:Str)
    # ...
  end
}
Cat.name = "Robbie"
```

Missing methods can be caught with `missing` and `set_missing` methods.
```
var Cat := {
  sub missing(method)
    # ...
  end
  sub set_missing(method, value)
    # ...
  end
}
Cat.length
Cat.length = 5
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

Arguments can be passed by name.
```
Cat.say(message = "meow")
```

### Extension Methods

Extension methods are methods that refer to another box.
When calling a method, extension methods take priority over normal methods.
```
var extensions = {
  sub Cat.say(message)
    # ...
  end
}
include extensions
Cat.say("nyan")
```

### Type Annotations

Type annotations are treated as methods, which are called every time they are used.

This allows you to pass them as variables:
```
sub increment(value:Num):value
  var big_value:value = value + 1
  return big_value
end
```

The `(of ..types)` operator (taken from Visual Basic) can be used on boxes. If not overloaded, the arguments can be retrieved with `generics()` or `generic(index)`.
```
var ToyBox := {
  var contents:generic(1)

  # Example overload
  sub of(types:Table):null
    origin(types)
  end
}

var toy_box = ToyBox(of int).new()
toy_box.contents = "ball" # error
```

Example using `self` as a type:
```
var Animal := {
  sub deep_fake():self
    return self.new
  end
}
var Cat := {
  include Animal
}
var fake_cat = Cat.deep_fake # fake_cat:Cat
```

Examples of typing tables:
```
var items = ["red", "blue", "green"](of int, string)
```
```
var items:Table(of int, string) = ["red", "blue", "green"]
```

### Casts

When a box is assigned to a variable of a different type, it is cast to that type.
```
var health:Decimal = 100
```

It does this by calling the `cast(to_type)` method.
```
sub cast(to_type)
  if to_type is decimal
    return to_dec()
  else
    return origin(to_type)
  end
end
```

### Tables

Tables are a type of box that store key-value pairs.
If the key is omitted, one-based indexing is used (like Lua).
They can be created with square brackets.
```
var nicknames := [
  "Kirito" = "Black Swordsman",
  "Asuna" = "Lightning Flash",
]

for nickname in nicknames
  log("\{nickname.key} a.k.a. \{nickname.value}")
end
```

Tables can be joined using the surplus operator.
```
var joined := [..table1, ..table2]
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
[deprecated(message:string? = null)] (variable, method) - warn if used
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
0 is_not integer    # not 0.includes(integer)
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

Holo uses enums as a type of box that can be created with `enum` or `Enum.new`. They contain a name:string and a value:number.
```
var entity_type := enum
  player = 1
  animal = 2
end

log(entity_type.get("player").name) # "player"
log(entity_type.get("animal").value) # 2

log(entity_type.animal.name) # calls missing method; works same as entity_type.get("animal").name
```

### Events

Events can be awaited and connected easily.
```
var on_fire := Event.new()
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
log(2) # Actor is locked between log(1) and log(2)
wait()
log(3)
```

If you need to lock over asynchronous methods, you can use a mutex, which limits the number of calls that can run at the same time.
```
var mutex = Mutex.new(1)
mutex.run(sub()
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

For simplicity, generic types (`(of ...)`) have not been annotated.

#### Box

Every box includes box, even if not in the `components` table.
- `stringify():Str` - returns "box"
- `new(..params):self` - creates a new box, adding this box as a component, setting `class` to return this box, and calling `init(params)`
- `init(..params):null` - default initialise method
- `class():Box` - returns self
- `generics():Table` - returns the generic types
- `generic(index:Box):Box` - returns the generic type at the given index or null
- `components():Table` - returns the boxes included in this box
- `variables():Table` - returns a table of [name, variable]
- `methods():Table` - returns a table of [name, delegate] (includes extension methods)
- `variable(name:Str):Variable` - returns the variable with the given name
- `method(name:Str):Delegate` - returns the method with the given name
- `eval(code:Delegate):Box` - executes the method in the box context
- `eval(code:Str):Box` - parses and executes the code in the box context
- `hash_code():Int` - returns a lookup number
- `==(other:Box):Bool` - returns true if both boxes have the same reference
- `!=(other:Box):Bool` - calls `==` and inverses with `not`
- `<=>(other:Box):Bool` - calls comparison operators (may be redundant, only implement if useful for table lookups)

#### Null (null)

A box representing no box.
- `stringify():Str` - returns "null"

#### Global

A box containing methods that can be called as if they were in `self`.
- `stringify():Str` - returns "global"
- `log(..messages):null` - logs each message to the standard output
- `warn(..messages):null` - logs each warning message to the standard output
- `throw(message:Box):null` - creates an exception and throws it
- `throw(exception:Exception):null` - throws the exception
- `input():Str` - reads and returns a line of user input
- `wait(duration:Num = 0.001):Dec` - yields for the duration in seconds and returns the exact amount of time waited
- `local_variables():Table` - returns a table of [name, variable]
- `rand(range1:Range):Num` - calls `random.rand` and returns a random number in the range
- `rand(max:Num):max` - calls `random.rand` and returns a random number in the range
- `exit(code:Int = 0):null` - exits the application with the given exit code
- `quit(code:Int = 0):null` - calls `exit`
- `holo_version():Str` - returns the Holo language version
- `holo_copyright():Str` - returns the Holo language copyrights

#### Boolean (Bool)

Has two instances: `true` and `false`.
- `stringify():Str` - returns "true" if equals `true`, otherwise "false"

#### String (Str) (includes Sequence)

An immutable sequence of characters.
- `stringify():Str` - returns self
- `count():Int` - returns the number of characters
- `count(sequence:Str):Int` - returns the number of times the sequence appears
- `length():Int` - calls `count()`
- `characters():Table` - gets a table of characters in the string
- `trim(predicate:Delegate? = null):Str` - removes characters matching a predicate from the start and end of the string (defaults to whitespace)
- `trim_start(predicate:Delegate? = null):Str` - removes characters matching a predicate from the start of the string (defaults to whitespace)
- `trim_start(sequence:Str):Str` - removes the sequence from the start of the string
- `trim_end(predicate:Delegate? = null):Str` - removes characters matching a predicate from the end of the string (defaults to whitespace)
- `trim_end(sequence:Str):Str` - removes the sequence from the end of the string
- `to_case(case:StringCaseType):Str` - converts the string to PascalCase, lowerCamelCase, snake_case, kebab-case, flatcase, Title Case, Sentence case
- `to_upper():Str` - converts letters in the string to uppercase
- `to_lower():Str` - converts letters in the string to lowercase
- `replace(find:Str, with:Str, limit:Int? = null):Str` - replaces each appearance of a sequence with another sequence up to limit times
- `insert(sequence:Str, position:Int):Str` - inserts the sequence at the position
- `split(separator:Str):Table` - separates the string into a table of strings
- `split():Table` - separates the string by whitespace into a table of strings
- `remove_range(between:Range):Str` - removes characters within the range
- `+(other:Box):Str` - concatenates the string and other.stringify
- `*(count:Int):Str` - repeats the string count times
- `==(other:Box):Bool` - returns true if other is an equal string
- `<(other:Box):Bool` - returns true if other is a string preceding alphabetically
- `<=(other:Box):Bool` - returns true if other is a string equal or preceding alphabetically
- `>=(other:Box):Bool` - returns true if other is a string succeeding alphabetically
- `>(other:Box):Bool` - returns true if other is a string equal or succeeding alphabetically

#### [abstract] Number (Num)

The base component for integers and decimals.
- `stringify():Str` - returns "number"
- `to_dec():Dec` - converts the number to a decimal
- `convert_angle(from:AngleType, to:AngleType):Num` - converts the angle between different types (degrees, radians, gradians, turns)
- `sqrt():Num` - returns the square root of the number
- `cbrt():Num` - returns the cube root of the number
- `lerp(to:Num, weight:Num):Dec` - linearly interpolates the number
- `abs():Num` - returns the positive value of the number
- `clamp(min:Num, max:Num):Num` - returns min if < min, max if > max, otherwise self
- `floor():Int` - returns the highest integer below the number
- `ceil():Int` - returns the lowest integer above the number
- `truncate():Int` - removes the decimal places of the number

#### Integer (Int)

A signed whole number with arbitrary size and precision.
- `stringify():Str` - returns the integer as a string
- `parse(str:Str):Int` - converts the string to an integer or throws
- `parse_or_null(str:Str?):Int?` - converts the string to an integer or returns null
- `Infinity():Int` - returns infinity
- `NaN():Int` - returns not-a-number

#### Decimal (Dec)

A signed fractional number with arbitrary size and precision.
- `stringify():Str` - returns the decimal as a string
- `parse(str:Str):Dec` - converts the string to a decimal or throws
- `parse_or_null(str:Str?):Dec?` - converts the string to a decimal or returns null
- `Infinity():Dec` - returns infinity
- `NaN():Dec` - returns not-a-number

#### Iterator

Gets each item in a sequence.
- `stringify():Str` - returns "iterator"
- `current():Entry` - returns the current item in the sequence
- `move_next():Bool` - tries to increment the sequence position

#### Entry

A key-value pair.
- `stringify():Str` - returns (key + " = " + value)
- `key():Box` - returns the key
- `set_key(key:Box):null` - sets the key
- `value():Box` - returns the value
- `set_value(value:Box):null` - sets the value

#### Sequence

An iterable, deferred sequence of key-value pairs.
Methods such as `append` also have a matching `with_append` which returns a new sequence.
- `stringify():Str` - returns "sequence"
- `each():Iterator` - returns an iterator for each item
- `to_table():Table` - adds each item to a new table
- `all(predicate:Delegate):Bool` - returns true if all items match the predicate
- `any(predicate:Delegate):Bool` - returns true if any item matches the predicate
- `any():Bool` - returns true if the sequence has at least one item
- `count():Int` - returns the number of items
- `count(item:Box):Int` - returns the number of times the item appears
- `length():Int` - calls `count()`
- `contains(item:Box):Bool` - returns true if sequence contains item
- `first(predicate:Delegate):Box` - returns the first item matching the predicate or throws
- `first_or_null(predicate:Delegate):Box?` - returns the first item matching the predicate or null
- `first():Box` - returns the first item or throws
- `first_or_null():Box` - returns the first item or null
- `last(predicate:Delegate):Box` - returns the last item matching the predicate or throws
- `last_or_null(predicate:Delegate):Box?` - returns the last item matching the predicate or null
- `last():Box` - returns the last item or throws
- `last_or_null():Box` - returns the last item or null
- `max(get_value:Delegate? = null):Num` - gets the biggest value in the sequence of numbers
- `min(get_value:Delegate? = null):Num` - gets the smallest value in the sequence of numbers
- `average(type:AverageType = AverageType.mean):Num` - returns the average value of the sequence of numbers using mean, median, mode, or range
- `sum():Num` - adds all items in the sequence of numbers
- `product():Num` - multiplies all items in the sequence of numbers
- `append(item:Box):null` - adds an item to the end
- `append_each(items:Sequence):null` - adds each item to the end
- `prepend(item:Box):Sequence` - adds an item to the start
- `prepend_each(items:Sequence):null` - adds each item to the start
- `concat(separator:str = "", stringify:Delegate? = null):str` - adds each item to a string by calling stringify
- `remove(item:Box, limit:Int? = null):null` - removes each appearance of the item up to limit times
- `remove_where(predicate:Delegate, limit:Int? = null):null` - removes items matching a predicate up to limit times
- `remove_first(count:Int = 1)` - removes the first count items
- `remove_last(count:Int = 1)` - removes the last count items
- `remove_duplicates():null` - removes duplicate items
- `clear():null` - removes all items
- `sort(comparer:Delegate? = null):null` - sorts the sequence into an order using the comparer
- `reverse():null` - reverses the order of the sequence
- `copy():Sequence` - shallow-copies the sequence into another sequence

#### Table (includes Sequence)

A sequence of key-value pairs.
- `stringify():Str` - returns a string like "[a = b, c = d]"
- `each():Iterator` - returns an iterator for each (key, value)
- `add(value:Box):null` - adds a value at the key one above the highest ordinal key
- `add_each(values:Table):null` - adds each value at the keys one above the highest ordinal key
- `set(entry:Entry):null` - adds an entry
- `set(key:Box, value:Box):null` - creates and adds an entry
- `set_each(values:Table):null` - sets each entry
- `get(key:Box):Box` - finds a value from the key or throws
- `get_or_null(key:Box?):Box?` - finds a value from the key or returns null
- `keys():null` - returns a table of keys
- `values():null` - returns a table of values
- `contains_key(key:Box?):Bool` - returns true if there's an entry with the given key
- `contains_value(value:Box?):Bool` - returns true if there's an entry with the given value
- `sample():Entry` - returns a random entry
- `shuffle():Entry` - randomises the keys
- `invert():null` - swaps the keys and values
- `weak():Bool` - returns true if the values are weakly referenced
- `set_weak(value:Bool):null` - references values weakly so they can be garbage collected
- `on_set():Event` - returns an event that's invoked when a value is set
- `on_get():Event` - returns an event that's invoked when a value is gotten

#### Range (includes Sequence)

A range between two inclusive numbers.
- `stringify():Str` - returns (min + " to " + max + " step " + step)
- `new(min:Num?, max:Num?, step:Num = 1):Range` - returns a new range
- `min():min` - returns the minimum value
- `set_min(value:Num?):null` - sets the minimum value
- `max():max` - returns the maximum value
- `set_max(value:Num?):null` - sets the maximum value
- `step():step` - returns the step value
- `set_step(value:Num):null` - sets the step value

#### Delegate

A box containing a method and a target.
- `stringify():Str` - returns (`target.stringify` + "." + `method_name`)
- `call(..arguments):Box?` - calls the best overload on the target
- `overloads():Table` - returns a table of method overloads
- `target():Box` - returns the method target
- `set_target(target:Box):null` - sets the method target
- `method_name():Str` - returns the method name
- `set_method_name(name:Str):null` - sets the method name

#### Variable

A box containing information about a variable.
- `stringify():Str` - returns (``)
- `name():Str` - returns the variable name
- `value():Box?` - returns the variable value
- `types():Table` - returns the allowed types of the variable

#### Time

A date and time in the Gregorian calendar.
- `stringify(format:Str):Str` - returns the time formatted with the given (.NET) format string
- `stringify():Str` - returns the time formatted like "2024/08/04 15:46 +0000"
- `new(total_seconds:Num)` - returns a new time
- `new(year:Num, month:Num, day:Num, hour:Num, minute:Num, second:Num, offset:Num = 0)` - returns a new time
- `new(year:Num, month:Num, day:Num, offset:Num = 0)` - returns a new time
- `now(offset:Num):Time` - returns the current time at the given offset
- `now():Time` - returns the current time at the local system offset
- `total_seconds():Dec` - returns the number of seconds since 0
- `total_milliseconds():Dec` - returns the number of milliseconds since 0
- `year():Int` - returns the year component
- `set_year(value:Num):null` - sets the year component
- `month():Int` - returns the month component
- `set_month(value:Num):null` - sets the month component
- `day():Int` - returns the day component
- `set_day(value:Num):null` - sets the day component
- `hour():Dec` - returns the hour component
- `set_hour(value:Num):null` - sets the hour component
- `minute():Dec` - returns the minute component
- `set_minute(value:Num):null` - sets the minute component
- `second():Dec` - returns the second component
- `set_second(value:Num):null` - sets the second component
- `offset():Dec` - returns the offset component
- `set_offset(value:Num):null` - sets the offset component
- `parse(time:Str):Time` - converts the string to a time or throws
- `parse_or_null(time:Str?):Time?` - converts the string to a time or returns null

#### Span

A period of time.
- `stringify(format:Str):Str` - returns the span formatted with the given (.NET) format string
- `stringify():Str` - returns the span formatted like "00:14:23.1294"
- `new(total_seconds:Num)` - returns a new span
- `new(hours:Num, minutes:Num, seconds:Num)` - returns a new span
- `total_seconds():Dec` - returns the total number of seconds
- `total_milliseconds():Dec` - returns the total number of milliseconds
- `hour():Dec` - returns the hour component
- `set_hour(value:Num):null` - sets the hour component
- `minute():Dec` - returns the minute component
- `set_minute(value:Num):null` - sets the minute component
- `second():Dec` - returns the second component
- `set_second(value:Num):null` - sets the second component
- `parse(span:Str):Span` - converts the string to a span or throws
- `parse_or_null(span:Str?):Span?` - converts the string to a span or returns null

#### Exception

An error or control code thrown up the call stack.
- `stringify():Str` - returns (`message()` + "\n" + `strack_trace()`)
- `new(message:Box? = null)` - returns an exception instance with the given message
- `message():Box` - returns the exception message
- `set_message():Str` - sets the exception message
- `stack_trace():Table` - returns each line of the call stack (including external lines from C#)

#### WeakReference (WeakRef)

Holds a reference to a box without preventing it from being garbage collected.
- `stringify():Str` - returns (`message()` + "\n" + `stack_trace()`)
- `target():Box` - returns the weakly referenced box
- `set_target(value:Box):null` - sets the weakly referenced box
- `is_alive():Bool` - returns true if the weak reference is still valid

#### Thread

Runs code in the background collaboratively.
- `run(method:Delegate, arguments:Table = []):Thread` - calls a method in the thread
- `wait():null` - waits for the thread to finish

#### Mutex

Limits the number of threads that can run at once.
- `new(limit:Int = 1):Mutex` - returns a mutex with the given entry limit
- `run(method:Delegate, arguments:Table = []):Thread` - calls a method in the mutex
- `limit():Int` - returns the entry limit
- `set_limit(limit:Int):null` - sets the entry limit
- `remaining():Int` - returns the remaining entries
- `set_remaining(remaining:Int):null` - sets the remaining entries

#### Canceller

Cancels a background task collaboratively.
- `cancel(delay:Num = 0):null` - calls `on_cancel()` after the delay
- `on_cancel():Event` - an event to be invoked when cancelled
- `is_cancelled():Bool` - returns true if cancelled
- `reset():null` - un-cancels the canceller for reuse
- `combined_with(other:canceller):Canceller` - combines two cancellers into one

#### Event

A signal to be awaited and listened to.
- `invoke(arguments:Table = []):null` - calls each listener and waiter
- `hook(method:Delegate, limit:Int? = null):null` - calls the method when the event is invoked up to limit times
- `wait():Table` - waits for the event to be invoked

#### Math

A collection of nerdy maths methods.
- `PI():Dec` - returns many digits of π (3.14...)
- `TAU():Dec` - returns many digits of τ (6.28...)
- `E():Dec` - returns many digits of e (2.71...)
- `sin`, `cos`, `tan`, `asin`, `acos`, `atan`, `atan2`, `sinh`, `cosh`, `tanh`, `asinh`, `acosh`, `atanh`, `exp`, `log`, `log10`, `log2`, `hypot` - who even cares about these

#### File

Access files on the hard drive.
- `read(path:Str):Str` - opens and reads text from the file
- `read_bytes(path:Str):Table` - opens and reads a table of bytes from the file
- `read_sequence(path:Str):Sequence` - opens and reads a sequence of bytes from the file
- `write(path:Str, value:Str):null` - opens and writes text to the file
- `write_bytes(path:Str, value:Table):null` - opens and writes a table of bytes to the file
- `write_sequence(path:Str, value:Table):Sequence` - opens and writes a sequence of bytes to the file
- `append(path:Str, value:Str):null` - opens and appends text to the file
- `append_bytes(path:Str, value:Table):null` - opens and appends a table of bytes to the file
- `append_sequence(path:Str, value:Sequence):Str` - opens and appends a sequence of bytes to the file
- `delete(path:Str):Bool` - deletes the file
- `exists(path:Str):Bool` - returns true if the file exists

#### Folder

Access folders/directories on the hard drive.
- `file_names(path:Str):Table` - returns a table of file names
- `file_names_recursive(path:Str):Table` - returns a table of file names, including files in nested folders
- `folder_names(path:Str):Table` - returns a table of folder names
- `folder_names_recursive(path:Str):Table` - returns a table of folder names, including folders in nested folders
- `delete(path:Str):Bool` - deletes the folder
- `exists(path:Str):Bool` - returns true if the folder exists

#### Path

Handle file paths.
- `join(a:Str, b:Str):Str` - joins two paths into one
- `file(path:Str, extension:Bool = true):Str` - returns the file name from the path (e.g. "C://Documents/neko.jpg" becomes "neko.jpg")
- `folder(path:Str):Str` - returns the folder path from the path (e.g. "C://Documents/neko.jpg" becomes "C://Documents")
- `extension(path:Str):Str` - returns the extension from the path (e.g. "C://Documents/neko.jpg" becomes "jpg")
- `trim_extension(path:Str):Str` - removes the path without the extension (e.g. "C://Documents/neko.jpg" becomes "C://Documents/neko")
- `drive(path:Str):Str?` - returns the drive from the path (e.g. "C://Documents/neko.jpg" becomes "C")
- `to_absolute(relative:Str):Str` - converts the relative path to an absolute path
- `to_relative(absolute:Str):Str` - converts the absolute path to a relative path

#### Random

Generate pseudo-random numbers.
- `seed():Num` - returns the random seed
- `set_seed(value:Num):null` - sets the random seed
- `int(min:Num, max:Num):Int` - returns a random integer from min to max
- `int(max:Num):Int` - calls `int(1, max)`
- `int(range:Range):Int` - returns a random integer in the range
- `dec(min:Num, max:Num):Dec` - returns a random decimal from min to max
- `dec(range:Range):Dec` - returns a random decimal in the range

### Versioning Guide

Holo uses versions like "1.0" and "2.4".

For developers:
- Increment the major version when adding new features or making breaking changes.
- Increment the minor version when fixing bugs or making small improvements.

For users:
- You usually want the latest major version, although it may require some changes to your project.
- You always want the latest minor version, and there should not be any issues upgrading.

### Style Guide

#### Naming

Names should be clear and concise.

Variables should use lower_snake_case:
```
var health := 100
var armor := 50
```

Constant variables should use PascalCase:
```
var Player := {
  var health := 100
}
```

Methods should use lower_snake_case unless they fetch a constant:
```
sub heal(amount:int) do
  health += amount
end
```
```
sub PI()
  return 3.14159265
end
```

##### Avoid redundant words

Avoid:
```
get_hash_code()
return_username()
remove_item(item)
```

Instead:
```
hash_code()
username()
remove(item)
```

##### Avoid misleading names for booleans

Avoid:
```
var freeze_objects:bool
var are_objects_frozen:bool
```

Instead:
```
var freeze_objects_on:bool
var freeze_objects_enabled:bool
```

##### Avoid misleading names for chain methods

Avoid:
```
var numbers := [1, 2].add(3)
```

Instead:
```
var numbers := [1, 2].with_add(3)
```

##### Naming collisions

Use a number suffix or descriptive name to avoid naming collisions.

Avoid:
```
for i in 1 to 5 do
  for j in 1 to 5 do
    for k in 1 to 5 do
```

Instead:
```
for i in 1 to 5 do
  for i2 in 1 to 5 do
    for i3 in 1 to 5 do
```
```
for x in 1 to 5 do
  for y in 1 to 5 do
    for z in 1 to 5 do
```

Avoid:
```
var cat := Cat.new
var cat1 := Cat.new
var kitty := Cat.new
```

Instead:
```
var cat := Cat.new
var cat2 := Cat.new
var cat3 := Cat.new
```
```
var red_cat := Cat.new
var green_cat := Cat.new
var blue_cat := Cat.new
```

#### Comments

Comments should be clear and concise.

##### Separating sections of code

Insert a comment on its own line before each section:
```
# Players
john.respawn
mikasa.respawn

# Enemies
dullahan.respawn
```

##### Explaining sections of code

Insert a comment on its own line before the section:
```
# Respawn player
player.dead = false
player.health = 100
player.teleport(0, 0)
```

##### Explaining specific lines in sections of code

Put a comment at the end of the line:
```
var shield := 50 # Percentage of health
```

##### Avoid magic numbers

Avoid:
```
# Create a 3x3 game grid
for i in 1 to 3 do
  log("###")
end
```

Instead:
```
# Create a game grid
for i in 1 to 3 do
  log("###")
end
```