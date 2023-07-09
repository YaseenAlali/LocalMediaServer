from youtubesearchpython import VideosSearch
from sys import argv
from os import chdir,path
from pyperclip import copy


"""
usage:
    python3 YTsearch.py searchterm limitv

    searchterm: the search term that you want to search for.. if multiple words are present use quotations
    limitv the number of videos to fetch, optional 


will save the links in search.txt and the first result to the clipboard

to be used on the GUI with YTDL
"""

if len(argv) == 2:
    searchterm = argv[1]
    limitv = 1
elif len(argv) == 3:
    searchterm = argv[1]
    limitv = int(argv[2])
else:
    print('invalid CLI args')
    exit()


path = path.abspath(__file__)
chdir(path.split('YTsearch.py')[0])
videosSearch = VideosSearch(searchterm, limit = limitv)
file = open('search.txt','w')

results = videosSearch.result()['result']
link = results[0]['link']

for result in results:
    file.write(result['link'] + '\n')
file.close()


copy(link)