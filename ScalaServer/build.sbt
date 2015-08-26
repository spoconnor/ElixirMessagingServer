name := "ScalaServer"

version := "1.0"

scalaVersion := "2.11.7"

resolvers += "Typesafe Repository" at "http://repo.typesafe.com/typesafe/releases/"

resolvers += "Akka Snapshot Repository" at "http://repo.akka.io/snapshots/"

libraryDependencies += "com.typesafe.akka" %% "akka-actor" % "2.4-SNAPSHOT"

libraryDependencies += "com.typesafe.akka" %% "akka-slf4j" % "2.4-SNAPSHOT"

libraryDependencies += "net.sandrogrzicic" %% "scalabuff-runtime" % "1.4.0"

libraryDependencies += "net.sandrogrzicic" %% "scalabuff-compiler" % "1.4.0"

