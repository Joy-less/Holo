# Holo

Holo is a scripting language inspired by C#, C++, Lua and Ruby. Its design philosophy:
- Consistency
- Embeddability
- Flexibility

## Examples

### FizzBuzz

```holo
null fizzbuzz(int n) {
    for (int i in 1 to 10) {
        if (i % 3 == 0 and i % 5 == 0) {
            log("FizzBuzz")
        }
        else if (i % 3 == 0) {
            log("Fizz")
        }
        else if (i % 5 == 0) {
            log("Buzz")
        }
        else {
            log(i)
        }
    }
}
```

### Fibonacci Sequence

```holo
int fibonacci(int n) {
    if (n in [0, 1]) return n
    return fibonacci(n - 1) + fibonacci(n - 2)
}
log(fibonacci(10))
```

## Design Proposal

### Overview

Every data type in Holo is an "object" ("obj").
Objects contain private variables and public methods.

## Concurrency & Parallelism

Holo uses actors and collaborative multithreading to prevent race conditions.

Actors can run in parallel and communicate by sending messages.

An actor can contain multiple coroutines, but only one coroutine can run at a time. The actor cycles between coroutines when they yield.

## Garbage Collection

Holo uses both "Mark & Sweep" and "Automatic Reference Counting" to collect dereferenced objects.
Both can be configured or disabled.

### Mark & Sweep

Each actor runs a simple "Mark & Sweep" algorithm:

- Each object has a state: pending, unmarked, marked.
- When an object is created, add it to a list of objects.
- When collecting garbage, set the state of each object to unmarked.
- Start at the actor, search variables recursively for objects and set their state to marked.
- Free each object whose state is unmarked.

The algorithm yields before processing each object to prevent pauses.

Justification: This algorithm detects cyclic references.

### Automatic Reference Counting

Each object runs a simple "Automatic Reference Counting" algorithm:

- Each object has a reference counter starting at 0.
- When a object is referenced, increment its reference counter.
- When a object is dereferenced, decrement its reference counter.
- Free a object when its reference counter reaches 0.

Justification: This algorithm provides better performance for short-lived objects.

## Methods



## Objects

Every object includes `object`, including `object`.

Objects contain:
- A table of variables, which are always private
- A table of methods, which are always public
- An internal array, which is used by `span`

## Collections

The base for all collections is `collection`.

Collections have an `int count()` method which returns a tracked value or enumerates the collection.
Collections have a `bool has_count()` method which returns whether the count is tracked.

### Spans

Spans include `collection` and represent a slice over a contiguous region of memory.

Spans have an `is_frozen()` method which returns whether the span's elements cannot be changed.
Spans have a `freeze()` method which prevents the span's elements from being changed.

### Lists

Lists include `collection` and are a wrapper over `span`.

It contains methods to add elements and automatically grows an internal span.

### Strings

Strings include `list` and are a wrapper over `span(byte)`.

Strings are assumed to be UTF-8 encoded.

### Tables

Tables include `list` and are a wrapper over `span(table_entry)`.

A `table_entry` contains a key and a value.
The key is hashed so that the span is binary-searchable.

### Sets

Sets include `list` and are a wrapper over `span(set_entry)`.

A `set_entry` contains a key.
The key is hashed so that the span is binary-searchable.

## Attributes

An attribute can be applied to any expression.

```holo
auto assign_increment = {
    include variable_assignment_attribute

    null before(variable_assignment_expression exp) {
        log("\{exp.variable_name} will be incremented")
    }
    null after(variable_assignment_expression exp) {
        exp.scope.eval("\{exp.name} += 1")
    }
}
assign_increment: int counter = 3
log(counter) // 4
```

- `override:` - Throws an exception if the method does not already exist
- 

## Operators

Every operator is converted to a method call which can be overridden.

| Operator | Name | Example | Equivalent | Output | Implementation |
| --- | --- | --- | --- | --- | --- |
| `not` | logical NOT | `not true` | ``true.`not`()`` | `false` | Checks for `false` |
| `and` | logical AND | `true and false` | ``true.`and`(false)`` | `false` | Checks both are `true` |
| `or` | logical OR | `true or false` | ``true.`or`(false)`` | `true` | Checks at least one is `true` |
| `~` | bitwise XOR | `true ~ false` | ``true.`~`(false)`` | `true` | Checks exactly one is `true`, evaluating both |
| `&` | bitwise AND | `true & false` | ``true.`&`(false)`` | `false` | Checks both are `true`, evaluating both |
| `\|` | bitwise OR | `true \| false` | ``true.`\|`(false)`` | `true` | Checks at least one is `true`, evaluating both |
| `>>` | bitwise RIGHT SHIFT | `10 >> 2` | ``10.`>>`(2)`` | `2` | Multiplies by `2**x` |
| `<<` | bitwise LEFT SHIFT | `10 << 2` | ``10.`<<`(2)`` | `40` | Divides by `2**x` |
| `~` | bitwise NOT | `~10` | ``10.`~`()`` | `-11` | Negates the number and subtracts `1` |
| `==` | equals | `true == false` | ``true.`==`(false)`` | `false` | Checks for equality |
| `~=` | not equals | `true ~= false` | ``true.`~=`(false)`` | `true` | Checks for no equality |
| `>` | greater than | `10 > 2` | ``10.`>`(2)`` | `true` | Checks for greater sort order |
| `<` | less than | `10 < 2` | ``10.`<`(2)`` | `false` | Checks for lesser sort order |
| `>=` | greater than or equal to | `10 >= 2` | ``10.`>=`(2)`` | `true` | Checks for greater or equal sort order |
| `<=` | less than or equal to | `10 <= 2` | ``10.`<=`(2)`` | `false` | Checks for lesser or equal sort order |
| `-` | unary minus | `-10` | ``10.`-`()`` | `-10` | Negates the value |
| `+` | unary plus | `+10` | ``10.`+`()`` | `10` | Keeps the value |
| `+` | add | `10 + 2` | ``10.`+`(2)`` | `12` | Calculates `a plus b` |
| `-` | subtract | `10 - 2` | ``10.`-`(2)`` | `8` | Calculates `a minus b` |
| `*` | multiply | `10 * 2` | ``10.`*`(2)`` | `20` | Calculates `a multiplied by b` |
| `/` | divide | `10 / 2` | ``10.`/`(2)`` | `5` | Calculates `a divided by b` |
| `%` | modulo | `10 % 2` | ``10.`%`(2)`` | `0` | Calculates the remainder of `a divided by b` |
| `**` | exponentiate | `10 ** 2` | ``10.`**`(2)`` | `100` | Calculates `a to the power of b` |
| `in` | contained in | `0 in [1, 2, 3]` | ``[1, 2, 3].contains(0)`` | `false` | Checks if `b contains a` |
| `not_in` | not contained in | `0 in [1, 2, 3]` | ``not [1, 2, 3].contains(0)`` | `false` | Checks if `b doesn't contain a` |
| `is` | includes | `0 is int` | ``0.includes(int)`` | `true` | Checks if `a includes b` |
| `is_not` | includes | `0 is_not int` | ``not 0.includes(int)`` | `false` | Checks if `a doesn't include b` |
| `??` | null coalesce | `3 ?? 5` | ``if (3 == null) 5 else 3`` | `3` | Selects the first non-`null` value |
| `?.` | null propagation | `3?.stringify()` | ``if (3 == null) null else 3.stringify()`` | `true` | Calls the method if not `null` |

Compound assignment operators are shorthand for applying operators to the current value.

| Operator | Equivalent |
| --- | --- |
| `name += value` | `name = name + value` |
| `name -= value` | `name = name - value` |
| `name *= value` | `name = name * value` |
| `name /= value` | `name = name / value` |
| `name %= value` | `name = name % value` |
| `name **= value` | `name = name ** value` |
| `name ??= value` | `name = name ?? value` |