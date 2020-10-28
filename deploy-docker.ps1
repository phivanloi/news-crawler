$ErrorActionPreference = "Stop"

docker rim news_crawler_beater_img
docker build -t news_crawler_beater_img -f Pl.Crawler.Beater\Dockerfile .
docker stop news_crawler_beater_container
docker rm news_crawler_beater_container
docker run --name news_crawler_beater_container -d news_crawler_beater_img

docker rim news_crawler_downloader_img
docker build -t news_crawler_downloader_img -f Pl.Crawler.Downloader\Dockerfile .
docker stop news_crawler_downloader_container
docker rm news_crawler_downloader_container
docker run --name news_crawler_downloader_container -d news_crawler_downloader_img

docker rim news_crawler_findlink_img
docker build -t news_crawler_findlink_img -f Pl.Crawler.Findlink\Dockerfile .
docker stop news_crawler_findlink_container
docker rm news_crawler_findlink_container
docker run --name news_crawler_findlink_container -d news_crawler_findlink_img

docker rim news_crawler_processer_img
docker build -t news_crawler_processer_img -f Pl.Crawler.Processer\Dockerfile .
docker stop news_crawler_processer_container
docker rm news_crawler_processer_container
docker run --name news_crawler_processer_container -d news_crawler_processer_img

docker rim news_crawler_exporter_img
docker build -t news_crawler_exporter_img -f Pl.Crawler.Exporter\Dockerfile .
docker stop news_crawler_exporter_container
docker rm news_crawler_exporter_container
docker run --name news_crawler_exporter_container -d news_crawler_exporter_img