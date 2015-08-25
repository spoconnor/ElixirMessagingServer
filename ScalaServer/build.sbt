name := "ScalaServer"
version := "1.0"
scalaVersion := "2.9.1"

resolvers += "Typesafe Repository" at "http://repo.typesafe.com/typesafe/releases/"
 
libraryDependencies += "com.typesafe.akka" % "akka-actor" % "2.0.5"

import scalabuff.ScalaBuffPlugin._

object build extends Build {
  lazy val root = Project("main", file("."), settings = Defaults.defaultSettings ++ scalabuffSettings).configs(ScalaBuff)
}
