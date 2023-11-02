# HOW TO ONLY CLONE PART OF A REPO
git clone --filter + git sparse-checkout downloads only the required files

https://stackoverflow.com/questions/600079/how-do-i-clone-a-subdirectory-only-of-a-git-repository

git clone --filter + git sparse-checkout downloads only the required files

E.g., to clone only files in subdirectory small/ in this test repository: https://github.com/cirosantilli/test-git-partial-clone-big-small-no-bigtree

git clone -n --depth=1 --filter=tree:0 \
  https://github.com/cirosantilli/test-git-partial-clone-big-small-no-bigtree
cd test-git-partial-clone-big-small-no-bigtree
git sparse-checkout set --no-cone small
git checkout
You could also select multiple directories for download with:

git sparse-checkout set --no-cone small small2
This method doesn't work for individual files however, but here is another method that does: How to sparsely checkout only one single file from a git repository?
