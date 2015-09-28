#!/usr/bin/env ruby

require 'curses'
include Curses

Curses.noecho
Curses.init_screen
Curses.stdscr.keypad(true)
Curses.start_color
Curses.init_pair(COLOR_WHITE,COLOR_WHITE,COLOR_BLACK)
Curses.init_pair(COLOR_BLUE,COLOR_BLUE,COLOR_BLACK)
Curses.init_pair(COLOR_RED,COLOR_RED,COLOR_BLACK)

class Spr
  attr_accessor :chr, :col
  def initialize(name, chr, col)
    @name = name
    @chr = chr
    @col = col
  end
  def draw(x,y)
puts "Draw #{chr} at #{x},#{y}"
    Curses.setpos(x,y)
    Curses.attron(color_pair(@col)|A_NORMAL) {
      Curses.addstr("#{@chr}")
    }
  end
end

class Sprites
  def initialize(file)
    @sprs = Hash.new()
    File.readlines(file).each do |line|
      args = line.split(',')
      id = args[0].to_i
      name = args[1]
      a = args[2]
      if (a.length == 1)
        chr = a
      else
        chr = a.force_encoding('utf-8')
      end
      c = args[3].to_i
      case (c)
      when 0
        col = COLOR_WHITE
      when 1
        col = COLOR_BLUE
      when 2
        col = COLOR_RED
      when 3
        col = COLOR_RED
      end
      @sprs[id] = Spr.new(name, chr, col)
    end
  end
  def get(id)
    @sprs[id]
  end
end

class Map
  def initialize()
    @sprs = Sprites.new("sprites.csv")
    @map = Array[
      Array[6,6,6,6,6],
      Array[6,3,3,2,6],
      Array[6,0,1,2,6],
      Array[0,0,0,2,4],
      Array[6,0,2,2,4],
      Array[6,6,4,4,4],
    ]
  end
  def draw()
    y=0
    @map.each do |line|
      y=y+1
      x=0
      line.each do |id|
        x=x+1
        @sprs.get(id).draw(x,y)
      end
    end
    Curses.refresh
  end
end

class Worker
  def initialize(index)
    @index = index
    @percent = 0
    @map = Map.new
  end

  def run
    @map.draw()
    #(1..10).each do
    #  work
    #  report
    #  sleep(rand())
    #end
  end

  #def to_s
  #  "Worker ##{'%2d' % @index} is #{'%3d' % @percent}% complete"
  #end

  #private

  #def work
  #  @percent += 10
  #end

  #def report
  #  Curses.attron(color_pair(COLOR_BLUE)|A_NORMAL) {
  #    Curses.addstr("#{0x2588.chr('UTF-8')}#{132.chr('UTF-8')}#{133.chr('UTF-8')}")
  #  }
  #  Curses.setpos(@index, 0)
  #  Curses.addstr(to_s)
  #  Curses.refresh
  #end
end

#workers = (1..10).map{ |index| Worker.new(index) }
#
#at_exit do
#  workers.each{ |worker| puts worker }
#end
#
#workers.map{ |worker| Thread.new{ worker.run } }.each(&:join)

puts "Init"
worker = Worker.new(0)
worker.run()

sleep(5)
Curses.close_screen
puts "Done"
