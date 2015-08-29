package com.example.akkaTcpChat 

object misc
{
  class FloatArray(width:Int, height:Int)
  {
    private var myArray = Array.ofDim[Float](width,height)
    def Width():Int = width
    def Height():Int = height
    def Get(x:Int, y:Int):Float = myArray(x)(y)
    def Set(x:Int, y:Int, value:Float) = myArray(x)(y) = value
    def Map(fn:Float=>Float) = 
    {
      myArray = myArray.map(_.map(fn))
    }
    
    def Dump() =
    {
      for (y <- 0 to height-1)
      {
        for (x <- 0 to width-1)
        {
          Console.print(myArray(x)(y))
        }
        Console.println()
      }   
    }

  }
    
  class IntArray(width:Int, height:Int)
  {
    private var myArray = Array.ofDim[Int](width,height)    
    def Width():Int = width
    def Height():Int = height
    def Get(x:Int, y:Int):Int = myArray(x)(y)
    def Set(x:Int, y:Int, value:Int) = myArray(x)(y) = value
    def Map(fn:Int=>Int) = 
    {
      myArray = myArray.map(_.map(fn))
    }
    
    def Dump() =
    {
      for (y <- 0 to height-1)
      {
        for (x <- 0 to width-1)
        {
          Console.print(myArray(x)(y))
        }
        Console.println()
      }   
    }
  }
}

