   M S B A M L   `   `   `  ����B  >AISDE_1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:AISDE_1AISDE_1  5   	        AISDE_1.GraphDisplayWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:AISDE_1     5   	     � Title$  GraphDisplayWindow��5   	   $	��768��6$   $
��1200��61   .��5	      � -   5         �Name$ 	graphGrid��5	      $
��Left=�6   $
��1200��69   $	��768��6F   $	��Top=�6S   .+�5
   
   �� -   5         ��Name$ canvas��5
      $
��Left=�6!   $	��724��6<   $	��Top=�6I   $
��1190��6a   .+�5      �� -   5         ��Name$ 	mstButton��5      $	��MST��6(    �� Left$ 25��66   
 �� Top$ 35��6G   $��75��6W   6b   �� 5      -   5        $ 
pathButton��5      $��Dijkstra��6)   $ 25��6<   $ 67��6M   $��75��6]   6h   �� 5      -   5        $ floydPathButton��5      $��Floyd��6.   $ 25��6>   $	 100��6O   $��75��6`   6k   �� 5      -   5        $ randomCostsButton��5      $��Losowe wagi��60   $ 25��6F   $	 135��6W   $��75��6h   6s   �� 5      -   5         ��Name$ StartingSolutionTextBlock��5      $
 1062��6;   $
��Wrap=�6N   $��0��6b   $ 22��6k   $��60��6{   �� 5      -   5         ��Name$ StartingSolutionLabel��5      $'��!Koszt rozwiązania początkowego:��63   $	 832��6]   $ 17��6o   $	��195��6   �� 5      -	   5        $ CalculatedSolutionLabel��5      $*��$Koszt rozwiązania optymalizowanego:��65   $	 832��6c   $ 43��6u   �� 5      -
   5        $ CalculatedSolutionTextBox��5      $
 1062��6;   $
��Wrap=�6N   $ 48��6b   $��67��6r   $��0��6}   �� 5      -   5        $ IterationsLabel��5      $��Liczba iteracji:��6-   $	 832��6H   $ 67��6Z   �� 5      -   5        $ IterationsTextBlock��5      $
 1062��65   $
��Wrap=�6H   $ 74��6\   $��0��6l   �� 5      -   5        $ AcceptedLabel��5      $!��Zaakceptowane rozwiązania:��6+   $	 832��6P   $ 93��6b   �� 5      -   5        $ AcceptedTextBlock��5      $
 1062��63   $
��Wrap=�6F   $��0��6Z   $ 98��6c   5      5      5      