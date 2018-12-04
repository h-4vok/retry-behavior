# DataReaderExtensions

If you feel that the IDataReader interface could use some work, here is your change. DataReaderExtensions focuses on empowering the IDataReader interface through new method extensions that will make your code a lot cleaner.

# Simple Type Getters

This is a very simple library. Here is the summary of methods that it adds to the IDataReader interface.

Let's assume we have the following variable

```#
IDataReader reader;
```

We have methods for all types:

```C#
reader.GetString
reader.GetBoolean
reader.GetByte
reader.GetDecimal
reader.GetDouble
reader.GetFloat
reader.GetInt16
reader.GetInt32
reader.GetInt64
reader.GetDateTime
```

So let's suppose we have a column named "MyTableID" which is an integer, we can do:

```C#
var id = reader.GetInt32("MyTableID");
```

You might be thinking this is trivial, why do we have a library for this? Well, you would have to do this with an IDataReader.

```C#
var ordinal = reader.GetOrdinal("MyTableID");
var id = reader.GetInt32(ordinal);
```

Sure, you might put it in one line, but that's not clean.

# Nullable Type Getters

Now we have a more interesting case. We might have a value that might be NULLABLE, but on top of that, IDataReader returns DBNull instead of null directly. 

This is what your code would look like, assuming your field is a DateTime. I am not using var so it is clear which types we are handling.

```C#
int ordinal = reader.GetOrdinal("NullableDate");
if (reader.IsDBNull(ordinal))
    return;

DateTime value = reader.GetDateTime(ordinal);
```

I tried to be clean. But man that's a bunch of lines for such a simple operation! Let's see what happens when we use our library.

```C#
var value = reader.GetDateTimeNullable("NullableDate");
```

You're welcome.

The full list of Nullable methods:

```C#
reader.GetString
reader.GetBooleanNullable
reader.GetByteNullable
reader.GetDecimalNullable
reader.GetDoubleNullable
reader.GetFloatNullable
reader.GetInt16Nullable
reader.GetInt32Nullable
reader.GetInt64Nullable
reader.GetDateTimeNullable
```

reader.GetString is nullable by default, the library will handle this case for you always.

# Byte Array Getter

This is a special type of getter. If you use VARBINARY fields, or anything that returns an array of bytes, this method is for you!

Let's see what we would need to do to read a column named "BinaryData" which is of type VARBINARY(MAX) (typical scenario).

Assume a variable reader of type IDataReader.

We are not using var so the types are clear in this example.

```C#
int ordinal = reader.GetOrdinal("BinaryData");
if (reader.IsDBNull(ordinal))
    return default(byte[]);

long dataLength = reader.GetBytes(ordinal, 0, null, 0, 0);
byte[] bytes = new byte[dataLength];
int bufferSize = 1024;
long bytesRead = 0L;
int curPos = 0;
while (bytesRead < dataLength)
{
    bytesRead += reader.GetBytes(ordinal, curPos, bytes, curPos, bufferSize);
    curPos += bufferSize;
}

return bytes;
```

That's a bunch of work! Seems like .NET's System.Data is punishing us for saving binary data into a database. Well, thankfully our library fixes this problem.

```C#
return reader.GetBytes("BinaryData");
```

Again, you are welcome!

If you find cases where you need to return an array of bytes, but this method does not work, then please send me the column definition and what you are trying to do as an Issue in GitHub. I would love to create a solution for you.