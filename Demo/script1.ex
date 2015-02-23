
defmodule Demo1 do

  def counter(state) do
    IO.puts "State = #{state}"
    receive do
      {:add, x} -> new_state = state + x
      {:sub, x} -> new_state = state - x
    end
    counter(new_state)
  end

#pid = spawn(fn -> Demo1.counter(10) end)
#send pid, {:add, 2}


  def counter2(state) do
    IO.puts "State = #{state}"
    receive do
      {:set, new_value} -> 
        counter2(new_value)
      {:get, sender} -> 
        send sender, state
        counter2(state)
    end
  end


#pid = spawn(fn -> Demo1.counter2(4) end) 
#send pid, {:set, 5}
#send pid, {:get, self()}
#receive do
#  result -> IO.puts result
#end

 def calc({:sub, x, y}) do
   x-y
  end

 def calc({:add, x, y}) when x < 0 do
   -x + y
 end

 def calc({:add, x, y}), do: x+y


end


