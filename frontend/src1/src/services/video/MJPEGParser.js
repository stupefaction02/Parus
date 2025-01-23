import '../../utils/Array.js';

export default class MJPEGParser {

    running = false;

    position = 0;

    totalReadBytes = 0;

    boundaryIndex = -1;

    imageHeaderIndex = -1;

    remainingBytes = 0;

    imageBuffer = [];

    newFrame;

    data;

    JPEG_HEADER_BYTES = [0xFF, 0xD8, 0xFF];

    constructor(newFrame) {
        this.newFrame = newFrame;

        this.imageBuffer = new Array(1024 * 1024);
    }

    setData(data) {
        this.data = data;
    }

    processData() {

        while (this.running) {
            var readedBytes = 0;

            var buffer = new Array(1024);
            readedBytes = buffer.length;
            buffer = this.data.slice(0, buffer.length);

            Array.Copy(buffer, 0, this.imageBuffer, this.totalReadBytes, readedBytes);
            this.totalReadBytes += readedBytes;

            this.remainingBytes = (this.totalReadBytes - this.position);
            if (this.imageHeaderIndex == -1 && this.remainingBytes != 0) {

                this.remainingBytes = this.totalReadBytes - this.position;
                this.imageHeaderIndex = this.imageBuffer.FindSubArray(this.JPEG_HEADER_BYTES, this.position, this.remainingBytes);
                debugger
            }

            this.remainingBytes = (this.totalReadBytes - this.position);
            while (this.imageHeaderIndex != -1 && this.boundaryIndex == -1 && this.remainingBytes != 0) {
                debugger
                this.boundaryIndex = this.imageBuffer.FindSubArray(this.JPEG_HEADER_BYTES, this.position, this.remainingBytes);

                if (this.boundaryIndex == -1)
                    this.position = this.totalReadBytes;

                this.remainingBytes = (this.totalReadBytes - this.position);
            }

            if (this.boundaryIndex != -1 && this.imageHeaderIndex != -1)
            {
                debugger
                var length = this.boundaryIndex - this.imageHeaderIndex;

             //   Stream imageStream = new MemoryStream( imageBuffer, imageHeaderIndex, length);

                this.newFrame({});
            }
        }

    }

    start() {
        this.running = true;

        this.processData();
    }

    stop() {

    }
}