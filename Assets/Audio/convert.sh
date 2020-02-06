for i in *.mp3;
  do name=`echo "$i" | cut -d'.' -f1`
  echo "$name"
  ffmpeg -i "$i" "${name}.ogg"
done
Try
    For Each f In Directory.GetFiles("F:\", "*.mp3", SearchOption.AllDirectories)
        File.Delete(f)
    Next
Catch ex As UnauthorizedAccessException
End Try