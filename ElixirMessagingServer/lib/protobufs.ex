defmodule CommsMessages do
#  use Protobuf, from: Path.expand("../../ProtoBufs/CommsMessages.proto", __DIR__)

  defmodule Header do
    defstruct(
      msgtype: "",
        # Response = 1;
        # Ping = 2;
        # Pong = 3;
        # NewUser = 4;
        # Login = 5;
        # Say = 6;
      from: "",
      dest: ""
    )
  end

  defmodule Ping do
    defstruct(
      count: 0
    )
  end
  defmodule Pong do
    defstruct(
      count: 0
    )
  end

  defmodule Response do
    defstruct(
      code: 0,
      message: ""
    )
  end

  defmodule NewUser do
    defstruct(
      name: "",
      username: "",
      password: ""
    )
  end
  
  defmodule Login do
    defstruct(
      username: "",
      password: ""
    )
  end
  
  defmodule Say do
    defstruct(
      text: ""
    )
  end
  
end
