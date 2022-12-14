# DirectoryScanner
Необходимо реализовать графическую утилиту с использованием WPF для анализа соотношения размера файлов и каталогов внутри выбранного каталога в многопоточном режиме.

Пользовательский интерфейс
----------------------------
Содержимое загруженной папки (выбирается через стандартный диалог открытия папки) должно быть представлено в иерархическом виде (элемент управления TreeView), где для каждого элемента должен быть указан размер в байтах и процент занимаемого пространства относительно общего размера каталога, в котором находится элемент. Узлы, соответствующие папкам и файлам, должны иметь разные иконки. Пример:

![image](https://user-images.githubusercontent.com/91383472/198673319-d4af47f3-a7c9-4097-93a3-1ad63c81c281.png)

Также необходимо реализовать функцию "отмены" сканирования на основе cancellation token (см. Cancellation in Managed Threads). При нажатии кнопки "Отмена" в дереве результатов достаточно отображать те сведения, которые были собраны к этому моменту.

Обязательным является применение MVVM, а также использование INotifyPropertyChanged и ICommand.

Анализ размера файлов и каталогов
----------------------------------
+ Анализ размера файлов и каталогов должен выполняться в многопоточном режиме с использованием системного пула потоков и очереди. 
+ Обработка каждого каталога выполняется в отдельном потоке. Обработка включает в себя суммирование размеров вложенных файлов и постановку в очередь всех вложенных каталогов для аналогичной обработки (см. ниже секцию "Важно").
+ Максимальное количество задействованных потоков должно быть ограничено (можно константой в коде) без изменения настроек системного пула потоков (использовать ThreadPool.SetMaxThreads запрещается).

Поток, обрабатывающий каталог, не должен дожидаться обработки всех вложенных каталогов, а только ставить их обработку в очередь. В противном случае при высоком уровне вложенности потоки будут простаивать впустую, дожидаясь завершения работы потоков, запущенных для вложенных каталогов.

Также при установке ограничения количества задействованных потоков в значение, меньшее уровня вложенности каталогов в сканируемом каталоге, возможно "зависание" программы из-за бесконечного ожидания (взаимной блокировки), когда все потоки заняты ожиданием завершения обхода вложенных каталогов, для запуска обработки которых потоков уже не остается.

Для решения этой проблемы перерасчет размеров каталогов с учётом вложенных каталогов нужно выполнять отдельно, уже после того, как размеры всех файлов посчитаны.
Для корректного определения общего размера каталога необходимо учитывать символические ссылки. 

Символические ссылки указывают на другие существующие файлы или папки и при подсчёте размера учитываться не должны.

Организация кода
-----------------
Код лабораторной работы должен состоять из трех проектов:
+ библиотека, выполняющая сбор информации о каталоге и построение удобной для отображения структуры данных;
+ модульные тесты для главной библиотеки;
+ WPF-приложение, отображающее структуру каталогов.
