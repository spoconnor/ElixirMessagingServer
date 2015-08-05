object misc
{
  class FloatArray(width:Int, height:Int)
  {
    private var myArray = Array.ofDim[Float](width,height)
    def Width():Int = width
    def Height():Int = height
    def Get(x:Int, y:Int):Float = myArray(x)(y)
    def Set(x:Int, y:Int, value:Float) = myArray(x)(y) = value
    def Map(fn:(Float)=>(Float)) = myArray.map(i:Array[Float] => fn(i))
  }
    
  class IntArray(width:Int, height:Int)
  {
    private var myArray = Array.ofDim[Int](width,height)    
    def Width():Int = width
    def Height():Int = height
    def Get(x:Int, y:Int):Int = myArray(x)(y)
    def Set(x:Int, y:Int, value:Int) = myArray(x)(y) = value
  }
}

