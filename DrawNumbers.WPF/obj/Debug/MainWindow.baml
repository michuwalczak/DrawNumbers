   M S B A M L   `   `   `  ����J  FDrawNumbers.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DrawNumbers.WPFDrawNumbers.WPF  5   	        DrawNumbers.WPF.MainWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   T PMahApps.Metro, Version=2.0.0.0, Culture=neutral, PublicKeyToken=51482d6f650b2b3f6mah,http://metro.mahapps.com/winfx/xaml/controls  5   	   Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5   	   )localclr-namespace:DrawNumbers.WPF  5   	   -   5          � Title$  Draw numbers��5	   	   $	��300��6   $	��400��6+   .��5
      �� .+�5   
   � $��5q�6    � ColumnDefinitions 5      �� 5      $	��150 �6#   �� 5      $	��250 �6#    � RowDefinitions 5      �� 5      $��25 �6    �� 5      $��25 �6    �� 5      $��25 �6    �� 5      $��25 �6    �� 5      $��25 �6    �� 5      $��25 �6    �� 5      $��25 �6    �� 5      $��25 �6    �� 5      $��25 �6    .+�5      �� -   5         ��Name$ LowerBound_Label��5      $��0��6.   $��0��6>   $
��Auto��6K   $
��Auto��6Y   $��Lower bound��6f   $��0,0,0,0q�6|   $
��Left=�6�   �� 5      -   5         ��Name$ LowerBound_TextBox��5      $��1��62   $��0��6B   $
��Auto��6O   $��Center=�6\   $��Stretch=�6�   $��	0,0,18,24q�6�   $��2��6�   ��6�   �� *
LowerBound+�� 5      -   5        $ UpperBound_Label��5      $��0��6.   $��1��6>   $
��Auto��6K   $
��Auto��6Y   $��Upper bound��6f   $��0,0,0,0q�6|   $
��Left=�6�   �� 5      -   5        $ UpperBound_TextBox��5      $��1��62   $��1��6B   $
��Auto��6O   $��Center=�6\   $��Stretch=�6�   $��	0,0,18,24q�6�   $��2��6�   ��6�   �� *
UpperBound+�� 5!      -   5        $ AmountOfNumbers_Label��5!      $��0��63   $��2��6C   $
��Auto��6P   $
��Auto��6^   $��Amount of numbers��6k   $��0,0,0,0q�6�   $
��Left=�6�   �� 5"      -   5        $ AmountOfNumbers_TextBox��5"      $��1��67   $��2��6G   $
��Auto��6T   $��Center=�6a   $��Stretch=�6�   $��	0,0,18,24q�6�   $��2��6�   �� 5$      -   5        $ DrawnNumbers_Label��5$      $��0��60   $��3��6@   $
��Auto��6M   $
��Auto��6[   $��Drawn numbers��6h   $��0q�6�   $
��Left=�6�   �� 5%      -	   5         ��Name$ DrawnNumbers_TextBlock��5%      $��1��68   $��3��6H   $
��Auto��6U   $
��Auto��6c    �� TextAlignment$ Center=�6�   $��0,2,18,2q�6�   ��6p   �� *NumbersUsage+6�   �� 5'      -
   5        $ ProgressValue��5'      $��0��6/   $��4��6?   $
��Wrap=�6L   $
 Left=�6~   ��6`   �� *ProgresLabel+6~   � 5(      -   5         �Name$ ProgressBar��5(      $��1��6/   $��4��6?   $
��Auto��6L   $
��Auto��6Z      MahApps.Styles.ProgressBar#���   6g   $��0,5,18,5q�6�    � Value 6�   �� *
Progress+6�   �� 5*      -   5        $ DrawTime_Label��5*      $��0��6,   $��5��6<   $
��Auto��6I   $
��Auto��6W   $��Draw time [ms]��6d   $��0q�6}   $
��Left=�6�   �� 5+      -   5        $ DrawTime_TextBox��5+      $��1��62   $��5��6B   $
��Auto��6O   $
��Auto��6]   $
��Wrap=�6j   $ Center=�6�   $��0,2,18,2q�6�   ��6~   �� *
DrawTime+6�   �� 5-      -   5        $ SaveTime_Label��5-      $��0��6,   $��6��6<   $
��Auto��6I   $
��Auto��6W   $��Save time [ms]��6d   $��0q�6}   $
��Left=�6�   �� 5.      -   5        $ SaveTime_TextBox��5.      $��1��62   $��6��6B   $
��Auto��6O   $
��Auto��6]   $
��Wrap=�6j   $ Center=�6�   $��0,2,18,2q�6�   ��6~   �� *
SaveTime+6�   �� 50      -   5        $ TotalTime_Label��50      $��0��6-   $��7��6=   $
��Auto��6J   $
��Auto��6X   $��Total time [ms]��6e   $��0q�6   $
��Left=�6�   �� 51      -   5        $ TotalTime_TextBox��51      $��1��63   $��7��6C   $
��Auto��6P   $
��Auto��6^   $
��Wrap=�6k   $ Center=�6�   $��0,2,18,2q�6�   ��6   �� *	TotalTime+6�   52      �� 53   
   -   5        	 ��Name$	 RunDraw_Button��53      $��0��6)   $��4��69   $	��100��6F   $��20��6R   $
��Draw��6^   6m   54      55      