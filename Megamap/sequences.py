import itertools
import sys

num = int(sys.argv[1])

sequences = list(itertools.permutations(range(0, num)))
for sequence in sequences:
    print(', '.join(map(str, sequence)))
