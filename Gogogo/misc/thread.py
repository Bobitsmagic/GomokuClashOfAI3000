import threading
import time

class ThreadingExample(object):
    def __init__(self, interval = 1):
        self.interval = interval

        thread = threading.Thread(target = self.run, args = ())
        thread.daemon = True
        self.thread = thread
        #thread.start()

    def run(self):
        for x in range(3):
            print('Doing something imporant in the background')
            time.sleep(self.interval)
        return "Hiya"

example = ThreadingExample()
example.thread.start()
#example.thread.join()
time.sleep(3)
print('Checkpoint')
time.sleep(20)
print('Bye')
