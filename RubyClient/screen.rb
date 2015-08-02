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
  def initialize(chr, col)
    @chr = chr
    @col = col
  end
  def draw(x,y)
    Curses.setpos(x,y)
    Curses.attron(color_pair(@col)|A_NORMAL) {
      Curses.addstr("#{@chr}")
    }
  end
end

class Sprites
  def initialize()
    @sprs = Hash.new()
    @sprs[0] = Spr.new(' ', COLOR_WHITE)
    @sprs[1] = Spr.new('@', COLOR_RED)
    @sprs[2] = Spr.new('#', COLOR_WHITE)
    @sprs[3] = Spr.new('o', COLOR_WHITE)
    @sprs[4] = Spr.new('-', COLOR_BLUE)
  end
  def get(id)
    @sprs[id]
  end
end

class Map
  def initialize()
    @sprs = Sprites.new()
    @map = Array[
      Array[0,0,0,0,0],
      Array[0,0,0,2,0],
      Array[3,0,1,2,0],
      Array[0,0,0,2,4],
      Array[3,0,2,2,4],
      Array[0,0,4,4,4],
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
  end
end

class Worker
  def initialize(index)
    @index = index
    @percent = 0
    @map = Map.new
    Curses.refresh
  end

  def run
    @map.draw()
    #(1..10).each do
    #  work
    #  report
    #  sleep(rand())
    #end
  end

  def to_s
    "Worker ##{'%2d' % @index} is #{'%3d' % @percent}% complete"
  end

  private

  def work
    @percent += 10
  end

  def report
    Curses.attron(color_pair(COLOR_BLUE)|A_NORMAL) {
      Curses.addstr("#{0x2588.chr('UTF-8')}#{132.chr('UTF-8')}#{133.chr('UTF-8')}")
    }
    Curses.setpos(@index, 0)
    Curses.addstr(to_s)
    Curses.refresh
  end
end

#workers = (1..10).map{ |index| Worker.new(index) }
#
#at_exit do
#  workers.each{ |worker| puts worker }
#end
#
#workers.map{ |worker| Thread.new{ worker.run } }.each(&:join)

worker = Worker.new(0)
worker.run()

Curses.close_screen
