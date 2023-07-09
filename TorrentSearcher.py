from py1337x import py1337x
from sys import argv
from os import path, chdir

path = path.abspath(__file__)
chdir(path.split('TorrentSearcher.py')[0])


def search_torrents(name="", catogry=""):
	torents = py1337x()
	results = torents.search(name, category=catogry, sortBy='seeders', order='desc') 
	
	return (results['items'][0]['name'],torents.info(results['items'][0]['link'])['magnetLink'],results['items'][0]['seeders'])




if len(argv) >= 3:
	name = argv[1]
	catogry = argv[2]
elif len(argv) >=2:
	name = argv[1]
else:
	exit(0)

title,magnetLink,seeders = search_torrents(name, catogry)
with open("torrentTitle.txt","w") as f:
	f.write(f'{title}\n{seeders}')

with open("torrentLink.txt","w") as fi:
	fi.write(magnetLink)