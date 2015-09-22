
public static class StreamFactory {
	private static Stream _stream;
	
	public static Stream stream{
		get{
			if(_stream == default(Stream)){
				_stream = new Stream("one");
			}
			return _stream;
		}
	}
}
