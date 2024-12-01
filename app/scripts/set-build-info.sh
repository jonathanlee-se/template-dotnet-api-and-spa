version=$1
branch=$2
date=$3

echo Version: $version
echo Branch: $branch
echo Date: $date

dir=./dist/emissions-ui/browser/config
for entry in `ls $dir`; do
  sed -i -e "s/\"version\": \".*\"/\"version\": \"$version\"/g" $dir/$entry
  sed -i -e "s/\"branch\": \".*\"/\"branch\": \"$branch\"/g" $dir/$entry
  sed -i -e "s/\"date\": \".*\"/\"date\": \"$date\"/g" $dir/$entry
done
