FROM golang:latest 
RUN mkdir /app 
RUN go get github.com/dtzar/openapi/go
ADD . /app/ 
WORKDIR /app 
RUN go build -o main . 
CMD ["/app/main"]