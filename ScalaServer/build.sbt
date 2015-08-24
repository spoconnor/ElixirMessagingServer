name := "ScalaServer"
version := "1.0"
scalaVersion := "2.9.1"

resolvers += "Typesafe Repository" at "http://repo.typesafe.com/typesafe/releases/"
 
libraryDependencies ++== Seq(
  "com.typesafe.akka" % "akka-actor" % "2.0.5",
  "com.trueaccord.scalapb" % "sbt-scalapb" % "0.4.19"
)
