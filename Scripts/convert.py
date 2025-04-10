import os
import codecs

def convert_encoding(file_path):
    try:
        # 以GBK编码读取文件内容
        with codecs.open(file_path, 'r', encoding='gbk') as f:
            content = f.read()
        
        # 以UTF-8编码写入文件（覆盖原文件）
        with codecs.open(file_path, 'w', encoding='utf-8') as f:
            f.write(content)
        
        print(f"成功转换: {file_path}")
    except Exception as e:
        print(f"转换失败: {file_path} | 错误信息: {str(e)}")

def batch_convert(folder_path):
    # 遍历文件夹所有文件
    for root, dirs, files in os.walk(folder_path):
        for file in files:
            if file.endswith('.cs'):
                file_path = os.path.join(root, file)
                convert_encoding(file_path)

if __name__ == "__main__":
    # 指定目标文件夹路径
    target_folder = r"D:\Horizon\Projects\Ongoing\Formula_One_Multi_Agent\Assets"  # 请替换为实际路径
    batch_convert(target_folder)